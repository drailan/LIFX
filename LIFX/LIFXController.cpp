#ifdef _MSC_VER
#define _CRT_SECURE_NO_WARNINGS
#endif

#include "LIFXController.h"

#include <stdlib.h>
#include <winsock2.h>
#include <IPHlpApi.h>
#include <ws2tcpip.h>
#include <iphlpapi.h>
#include <stdio.h>
#include <clocale>
#include <cassert>

using namespace std;

namespace LIFX
{

	LIFXController::LIFXController()
	{
		setlocale(LC_CTYPE, "");
		bulbs = vector<lifx_bulb>();

		addr.sin_family = AF_INET;
		addr.sin_port = htons(DEFAULT_PORT);
	}

	void LIFXController::SetPower(const wchar_t* target, uint16_t value)
	{
		for (auto &bulb : bulbs) {
			if (wcscmp(bulb.label, target) == 0) {
				int iResult;

				auto packet = new lifx_header();
				memset(packet, 0, sizeof(lifx_header));
				packet->size = sizeof(lifx_header) + 6;
				packet->type = 0x0075;
				packet->protocol = _byteswap_ushort(0x14);

				FormMac(packet->target, bulb.mac);
				FormMac(packet->site, bulb.site_address);

				out_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
				inet_pton(AF_INET, bulb.ip, &(addr.sin_addr.s_addr));

				char buf[42];
				ZeroMemory(buf, sizeof(buf));
				memcpy(buf, packet, 36);
				buf[36] = value & 0xFF;
				buf[37] = value >> 8;

				iResult = sendto(out_socket, reinterpret_cast<const char*>(&buf), sizeof(buf), 0, reinterpret_cast<SOCKADDR *>(&addr), sizeof(addr));
				closesocket(out_socket);
			}
		}
	}

	uint16_t LIFXController::GetPower(const wchar_t* target)
	{
		for (auto &bulb : bulbs) {
			if (wcscmp(bulb.label, target) == 0) {
				int iResult;

				auto packet = new lifx_header();
				memset(packet, 0, sizeof(lifx_header));
				packet->size = sizeof(lifx_header);
				packet->type = 20;
				packet->protocol = _byteswap_ushort(0x34);

				FormMac(packet->site, bulb.site_address);

				out_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
				inet_pton(AF_INET, bulb.ip, &(addr.sin_addr.s_addr));

				iResult = sendto(out_socket, reinterpret_cast<const char*>(packet), sizeof(lifx_header), 0, reinterpret_cast<SOCKADDR *>(&addr), sizeof(addr));

				auto recvbuflen = 512;
				char recvbuf[512];

				addr.sin_addr.s_addr = INADDR_ANY;
				in_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
				iResult = bind(in_socket, reinterpret_cast<SOCKADDR *>(&addr), sizeof(addr));

				sockaddr_in from;
				int from_length = sizeof(sockaddr_in);

				do {
					iResult = recvfrom(in_socket, recvbuf, recvbuflen, 0, reinterpret_cast<SOCKADDR*>(&from), &from_length);
					if (iResult > 0) {
						break; // ой, все
					} else if (iResult == 0) {
						printf("Connection closed\n");
					} else {
						printf("recv failed: %d\n", WSAGetLastError());
					}
				} while (iResult > 0);

				closesocket(out_socket);
				closesocket(in_socket);

				return InvertAndConvertHexBufToUint(&recvbuf[36]);
			}
		}
	}

	void LIFXController::SetLightColor(const wchar_t* target, uint16_t* state)
	{
		for (auto &bulb : bulbs) {
			if (wcscmp(bulb.label, target) == 0) {
				int iResult;

				auto packet = new lifx_header();
				memset(packet, 0, sizeof(lifx_header));
				packet->size = sizeof(lifx_header) + 13;
				packet->type = 0x0066;
				packet->protocol = _byteswap_ushort(0x14);

				FormMac(packet->target, bulb.mac);
				FormMac(packet->site, bulb.site_address);

				out_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
				//addr.sin_addr.s_addr = inet_addr(bulb.ip);
				inet_pton(AF_INET, bulb.ip, &(addr.sin_addr.s_addr));

				char buf[49];
				ZeroMemory(buf, sizeof(buf));
				memcpy(buf, packet, 36);
				buf[36] = 0;

				buf[37] = state[0] & 0xFF;
				buf[38] = state[0] >> 8;

				buf[39] = state[1] & 0xFF;
				buf[40] = state[1] >> 8;

				buf[41] = state[2] & 0xFF;
				buf[42] = state[2] >> 8;

				buf[43] = state[3] & 0xFF;
				buf[44] = state[3] >> 8;

				buf[45] = state[4] & 0xFF;
				buf[46] = state[4] >> 8;

				iResult = sendto(out_socket, reinterpret_cast<const char*>(&buf), sizeof(buf), 0, reinterpret_cast<SOCKADDR *>(&addr), sizeof(addr));
				closesocket(out_socket);
			}
		}
	}

	void LIFXController::GetDiscoveryPacket(uint8_t seq, void* ptr)
	{
		auto packet = new lifx_header();
		memset(packet, 0, sizeof(lifx_header));
		packet->size = sizeof(lifx_header);
		packet->type = 2;
		packet->ack_required = 0;
		packet->res_required = 0;
		packet->sequence = seq;
		packet->source = 666;
		packet->protocol = _byteswap_ushort(0x34);

		memcpy(ptr, reinterpret_cast<char*>(packet), packet->size);
	}

	void LIFXController::GetLabelPacket(uint64_t site_address, uint8_t seq, void* ptr)
	{
		auto packet = new lifx_header();
		memset(packet, 0, sizeof(lifx_header));
		packet->size = sizeof(lifx_header);
		packet->type = 23;
		packet->ack_required = 0;
		packet->res_required = 0;
		packet->sequence = seq;
		packet->source = 666;
		packet->protocol = _byteswap_ushort(0x34);

		FormMac(packet->site, site_address);

		memcpy(ptr, reinterpret_cast<char*>(packet), packet->size);
	}

	void LIFXController::GetLightStatePacket(uint64_t site_address, uint8_t seq, void* ptr)
	{
		auto packet = new lifx_header();
		memset(packet, 0, sizeof(lifx_header));
		packet->size = sizeof(lifx_header);
		packet->type = 101;
		packet->ack_required = 0;
		packet->res_required = 0;
		packet->sequence = seq;
		packet->source = 666;
		packet->protocol = _byteswap_ushort(0x34);

		FormMac(packet->site, site_address);

		memcpy(ptr, reinterpret_cast<char*>(packet), packet->size);
	}

	void LIFXController::GetGroupPacket(uint64_t site_address, uint8_t seq, void* ptr)
	{
		auto packet = new lifx_header();
		memset(packet, 0, sizeof(lifx_header));
		packet->size = sizeof(lifx_header);
		packet->type = 51;
		packet->ack_required = 0;
		packet->res_required = 0;
		packet->sequence = seq;
		packet->source = 666;
		packet->protocol = _byteswap_ushort(0x34);

		FormMac(packet->site, site_address);

		memcpy(ptr, reinterpret_cast<char*>(packet), packet->size);
	}

	uint32_t LIFXController::InvertAndConvertHexBufToUint(char buf[2])
	{
		unsigned char data[2] = {
			static_cast<unsigned char>(buf[1]),
			static_cast<unsigned char>(buf[0])
		};
		char hex[5];

		sprintf(&hex[0], "%2x", data[0]);
		sprintf(&hex[2], "%2x", data[1]);

		uint32_t out;

		sscanf(hex, "%x", &out);
		return out;
	}

	LIFXController::~LIFXController()
	{
		WSACleanup();
		bulbs.clear();
	}

	void LIFXController::FormMac(uint8_t* target, uint64_t value)
	{
		// ignoring two zero bytes at the end because of Converter.ToInt64()
		target[0] = uint8_t((value >> 56) & 0xFF);
		target[1] = uint8_t((value >> 48) & 0xFF);
		target[2] = uint8_t((value >> 40) & 0xFF);
		target[3] = uint8_t((value >> 32) & 0xFF);
		target[4] = uint8_t((value >> 24) & 0xFF);
		target[5] = uint8_t((value >> 16) & 0xFF);
	}
}
