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

#include <iostream>
#include <string>

using namespace std;

namespace LIFX
{

	LIFXController::LIFXController()
	{
		setlocale(LC_CTYPE, "");
	}

	void LIFXController::SetLightColor(const wchar_t* target, uint16_t* state)
	{
		//for (auto &bulb : bulbs) {
		//	if (wcscmp(bulb.label, target) == 0) {
		//		int iResult;

		//		auto packet = new lifx_header();
		//		memset(packet, 0, sizeof(lifx_header));
		//		packet->size = sizeof(lifx_header) + 13;
		//		packet->type = 0x0066;
		//		packet->protocol = _byteswap_ushort(0x14);

		//		FormMac(packet->target, bulb.mac);
		//		FormMac(packet->site, bulb.site_address);

		//		out_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
		//		//addr.sin_addr.s_addr = inet_addr(bulb.ip);
		//		inet_pton(AF_INET, bulb.ip, &(addr.sin_addr.s_addr));

		//		char buf[49];
		//		ZeroMemory(buf, sizeof(buf));
		//		memcpy(buf, packet, 36);
		//		buf[36] = 0;

		//		buf[37] = state[0] & 0xFF;
		//		buf[38] = state[0] >> 8;

		//		buf[39] = state[1] & 0xFF;
		//		buf[40] = state[1] >> 8;

		//		buf[41] = state[2] & 0xFF;
		//		buf[42] = state[2] >> 8;

		//		buf[43] = state[3] & 0xFF;
		//		buf[44] = state[3] >> 8;

		//		buf[45] = state[4] & 0xFF;
		//		buf[46] = state[4] >> 8;

		//		iResult = sendto(out_socket, reinterpret_cast<const char*>(&buf), sizeof(buf), 0, reinterpret_cast<SOCKADDR *>(&addr), sizeof(addr));
		//		closesocket(out_socket);
		//	}
		//}
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

	void LIFXController::SetPowerPacket(uint64_t site_address, uint64_t mac, uint8_t seq, uint16_t power, void* ptr)
	{
		auto packet = new lifx_header();
		memset(packet, 0, sizeof(lifx_header));
		packet->size = sizeof(lifx_header);
		packet->type = 21;
		packet->ack_required = 0;
		packet->res_required = 1;
		packet->sequence = seq;
		packet->source = 666;
		packet->protocol = _byteswap_ushort(0x34);

		FormMac(packet->site, site_address);

		char buffer[38];
		ZeroMemory(buffer, sizeof(buffer));
		memcpy(buffer, packet, 36);
		buffer[36] = power & 0xFF;
		buffer[37] = power >> 8;

		memcpy(ptr, reinterpret_cast<char*>(buffer), sizeof(buffer));
	}

	void LIFXController::SetLabelPacket(uint64_t site_address, uint64_t mac, uint8_t seq, char* label, void* ptr)
	{
		auto packet = new lifx_header();
		memset(packet, 0, sizeof(lifx_header));
		packet->size = sizeof(lifx_header);
		packet->type = 24;
		packet->ack_required = 0;
		packet->res_required = 1;
		packet->sequence = seq;
		packet->source = 666;
		packet->protocol = _byteswap_ushort(0x34);

		FormMac(packet->site, site_address);

		char buffer[68];
		ZeroMemory(buffer, sizeof(buffer));
		memcpy(buffer, packet, 36);
		memcpy(&buffer[36], label, sizeof(label));

		memcpy(ptr, reinterpret_cast<char*>(buffer), sizeof(buffer));
	}

	LIFXController::~LIFXController()
	{
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
