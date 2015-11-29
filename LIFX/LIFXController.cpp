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
		count = 4;
		bulbs = vector<lifx_bulb>();
		init_network();

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

				packet->target[0] = static_cast<uint8_t>((bulb.mac >> 40) & 0xFF);
				packet->target[1] = static_cast<uint8_t>((bulb.mac >> 32) & 0xFF);
				packet->target[2] = static_cast<uint8_t>((bulb.mac >> 24) & 0xFF);
				packet->target[3] = static_cast<uint8_t>((bulb.mac >> 16) & 0xFF);
				packet->target[4] = static_cast<uint8_t>((bulb.mac >> 8) & 0xFF);
				packet->target[5] = static_cast<uint8_t>((bulb.mac)& 0xFF);

				auto controller_mac = bulb.site_address;
				packet->site[0] = static_cast<uint8_t>((controller_mac >> 40) & 0xFF);
				packet->site[1] = static_cast<uint8_t>((controller_mac >> 32) & 0xFF);
				packet->site[2] = static_cast<uint8_t>((controller_mac >> 24) & 0xFF);
				packet->site[3] = static_cast<uint8_t>((controller_mac >> 16) & 0xFF);
				packet->site[4] = static_cast<uint8_t>((controller_mac >> 8) & 0xFF);
				packet->site[5] = static_cast<uint8_t>((controller_mac)& 0xFF);

				out_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
				addr.sin_addr.s_addr = inet_addr(bulb.ip);

				char buf[42];
				ZeroMemory(buf, sizeof(buf));
				memcpy(buf, packet, 36);
				buf[36] = value & 0xFF;
				buf[37] = value >> 8;

				iResult = sendto(out_socket, reinterpret_cast<const char*>(&buf), sizeof(buf), 0, reinterpret_cast<SOCKADDR *>(& addr), sizeof(addr));
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

				packet->target[0] = static_cast<uint8_t>((bulb.mac >> 40) & 0xFF);
				packet->target[1] = static_cast<uint8_t>((bulb.mac >> 32) & 0xFF);
				packet->target[2] = static_cast<uint8_t>((bulb.mac >> 24) & 0xFF);
				packet->target[3] = static_cast<uint8_t>((bulb.mac >> 16) & 0xFF);
				packet->target[4] = static_cast<uint8_t>((bulb.mac >> 8) & 0xFF);
				packet->target[5] = static_cast<uint8_t>((bulb.mac) & 0xFF);

				auto controller_mac = bulb.site_address;
				packet->site[0] = static_cast<uint8_t>((controller_mac >> 40) & 0xFF);
				packet->site[1] = static_cast<uint8_t>((controller_mac >> 32) & 0xFF);
				packet->site[2] = static_cast<uint8_t>((controller_mac >> 24) & 0xFF);
				packet->site[3] = static_cast<uint8_t>((controller_mac >> 16) & 0xFF);
				packet->site[4] = static_cast<uint8_t>((controller_mac >> 8) & 0xFF);
				packet->site[5] = static_cast<uint8_t>((controller_mac)& 0xFF);

				out_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
				addr.sin_addr.s_addr = inet_addr(bulb.ip);

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
			}
		}
	}

	light_state LIFXController::GetLightState(const wchar_t* label)
	{
		for (auto &bulb : bulbs) {
			if (wcscmp(bulb.label, label) == 0) {
				int iResult;

				auto packet = new lifx_header();
				memset(packet, 0, sizeof(lifx_header));
				packet->size = sizeof(lifx_header);
				packet->type = 101;
				packet->protocol = _byteswap_ushort(0x34);

				auto mac = bulb.site_address;
				packet->site[0] = uint8_t((mac >> 40) & 0xFF);
				packet->site[1] = uint8_t((mac >> 32) & 0xFF);
				packet->site[2] = uint8_t((mac >> 24) & 0xFF);
				packet->site[3] = uint8_t((mac >> 16) & 0xFF);
				packet->site[4] = uint8_t((mac >> 8) & 0xFF);
				packet->site[5] = uint8_t((mac)& 0xFF);

				//packet->target[0] = static_cast<uint8_t>((bulb.mac >> 40) & 0xFF);
				//packet->target[1] = static_cast<uint8_t>((bulb.mac >> 32) & 0xFF);
				//packet->target[2] = static_cast<uint8_t>((bulb.mac >> 24) & 0xFF);
				//packet->target[3] = static_cast<uint8_t>((bulb.mac >> 16) & 0xFF);
				//packet->target[4] = static_cast<uint8_t>((bulb.mac >> 8) & 0xFF);
				//packet->target[5] = static_cast<uint8_t>((bulb.mac) & 0xFF);

				out_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
				addr.sin_addr.s_addr = inet_addr(bulb.ip);

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

				auto state = light_state();
				strcpy(state.label, &recvbuf[48]);
				state.power = InvertAndConvertHexBufToUint(&recvbuf[46]);
				state.dim = InvertAndConvertHexBufToUint(&recvbuf[44]);
				state.kelvin = InvertAndConvertHexBufToUint(&recvbuf[42]);
				state.brightness = InvertAndConvertHexBufToUint(&recvbuf[40]);
				state.saturation = InvertAndConvertHexBufToUint(&recvbuf[38]);
				state.hue = InvertAndConvertHexBufToUint(&recvbuf[36]);

				return state;
			}
		}
	}

	void LIFXController::init_network()
	{
		WSAStartup(MAKEWORD(2, 2), &wsaData);

		bcast_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
		char opt = 1;
		setsockopt(bcast_socket, SOL_SOCKET, SO_BROADCAST, static_cast<char*>(&opt), sizeof(char));

		out_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
		in_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
	}

	vector<wstring> LIFXController::GetLabels()
	{
		auto out = vector<wstring>();
		for (auto bulb : bulbs)
		{
			out.push_back(bulb.label);
		}

		return out;
	}

	void LIFXController::Discover()
	{
		int iResult;
		addr.sin_addr.s_addr = INADDR_BROADCAST;

		lifx_header* packet = new lifx_header();
		memset(packet, 0, sizeof(lifx_header));
		packet->size = sizeof(lifx_header);
		packet->type = 2;
		packet->protocol = _byteswap_ushort(0x34);

		iResult = sendto(bcast_socket, reinterpret_cast<const char*>(packet), sizeof(lifx_header), 0, reinterpret_cast<SOCKADDR *>(&addr), sizeof(addr));
		iResult = closesocket(bcast_socket);

		int recvbuflen = 512;
		char recvbuf[NUM_LAMPS][512];

		addr.sin_addr.s_addr = INADDR_ANY;
		iResult = bind(in_socket, reinterpret_cast<SOCKADDR *>(& addr), sizeof(addr));

		sockaddr_in from[NUM_LAMPS];
		int from_length = sizeof(sockaddr_in);

		auto i = 0;
		do {
			iResult = recvfrom(in_socket, recvbuf[i], recvbuflen, 0, reinterpret_cast<SOCKADDR*>(&from[i]), &from_length);
			if (iResult > 0) {
				++i;
				if (i == NUM_LAMPS)
					break; // ой, все
			} else if (iResult == 0) {
				printf("Connection closed\n");
			} else {
				printf("recv failed: %d\n", WSAGetLastError());
			}
		} while (iResult > 0);

		closesocket(in_socket);

		auto temp = recvbuf[0];
		char site_address[18];
		ZeroMemory(site_address, sizeof(site_address));

		sprintf(&site_address[0], "%02X", temp[16]);
		site_address[2] = ':';
		sprintf(&site_address[3], "%02X", temp[17]);
		site_address[5] = ':';
		sprintf(&site_address[6], "%02X", temp[18]);
		site_address[8] = ':';
		sprintf(&site_address[9], "%02X", temp[19]);
		site_address[11] = ':';
		sprintf(&site_address[12], "%02X", temp[20]);
		site_address[14] = ':';
		sprintf(&site_address[15], "%02X", temp[21]);

		auto site_address_proper = string_to_mac(site_address);

		for (i = 0; i < NUM_LAMPS; ++i) {
			bulbs.push_back(lifx_bulb());
			strcpy(bulbs[i].ip, inet_ntoa(from[i].sin_addr));
			bulbs[i].mac = get_mac(bulbs[i].ip);
			bulbs[i].site_address = site_address_proper;
			wcscpy(bulbs[i].label, get_label(bulbs[i]).c_str());
		}
	}

	wstring LIFXController::get_label(const lifx_bulb target)
	{
		int iResult;

		auto packet = new lifx_header();
		memset(packet, 0, sizeof(lifx_header));
		packet->size = sizeof(lifx_header);
		packet->type = 23;
		packet->protocol = _byteswap_ushort(0x34);

		auto mac = target.site_address;
		packet->site[0] = uint8_t((mac >> 40) & 0xFF);
		packet->site[1] = uint8_t((mac >> 32) & 0xFF);
		packet->site[2] = uint8_t((mac >> 24) & 0xFF);
		packet->site[3] = uint8_t((mac >> 16) & 0xFF);
		packet->site[4] = uint8_t((mac >> 8) & 0xFF);
		packet->site[5] = uint8_t((mac)& 0xFF);

		out_socket = socket(AF_INET, SOCK_DGRAM, IPPROTO_UDP);
		addr.sin_addr.s_addr = inet_addr(target.ip);

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

		char label[256];
		memcpy(label, &recvbuf[36], 256);
		auto str = string(label);

		auto wstr = wstring();
		wstr.assign(str.begin(), str.end());

		return wstr;
	}

	uint32_t LIFXController::InvertAndConvertHexBufToUint(char buf[2])
	{
		unsigned char data[2] = { buf[1], buf[0] };
		char hex[5];

		sprintf(&hex[0], "%2x", data[0]);
		sprintf(&hex[2], "%2x", data[1]);

		uint32_t out;

		sscanf(hex, "%x", &out);
		return out;
	}

	void LIFXController::get_bulb_info()
	{
		printf("Initial discovery data:\n");
		for (auto i = 0; i < NUM_LAMPS; ++i) {
			printf("Bulb: %d, ip is %s, mac is %016llX\n", i, bulbs[i].ip, bulbs[i].mac);
		}
	}

	LIFXController::~LIFXController()
	{
		WSACleanup();
		bulbs.clear();
	}

	uint64_t LIFXController::string_to_mac(const char* s)
	{
		unsigned char a[6];
		auto last = -1;
		auto rc = sscanf(s, "%hhx:%hhx:%hhx:%hhx:%hhx:%hhx%n",
			a + 0, a + 1, a + 2, a + 3, a + 4, a + 5,
			&last);
		if (rc != 6 || strlen(s) != last)
			return -1;
		return
			uint64_t(a[0]) << 40 |
			uint64_t(a[1]) << 32 |
			uint64_t(a[2]) << 24 |
			uint64_t(a[3]) << 16 |
			uint64_t(a[4]) << 8 |
			uint64_t(a[5]);
	}

	uint64_t LIFXController::get_mac(char* ip)
	{
		DWORD dRet;
		IPAddr src = 0;
		ULONG mac[2];
		ULONG len = 6;
		BYTE *address;
		char mac_string[50] = "";
		char temp_mac[6];
		auto dest = inet_addr(ip);

		ZeroMemory(&mac, sizeof(mac));
		dRet = SendARP(dest, src, &mac, &len);
		address = reinterpret_cast<BYTE *>(&mac);

		for (auto i = 0; i < len; i++) {
			if (i == (len - 1)) {
				sprintf(temp_mac, "%.2X", int(address[i]));
			} else {
				sprintf(temp_mac, "%.2X:", int(address[i]));
			}

			strncat(mac_string, temp_mac, strlen(temp_mac));
		}

		return string_to_mac(mac_string);
	}

	void LIFXController::print_hex_memory(void *mem)
	{
		int i;
		auto p = static_cast<unsigned char *>(mem);
		for (i = 0; i < 128; i++) {
			printf("0x%02x ", p[i]);
			if (i % 16 == 0)
				printf("\n");
		}
		printf("\n");
	}
}
