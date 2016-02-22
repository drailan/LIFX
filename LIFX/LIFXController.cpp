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

		MacFromUint64_t(packet->site, site_address);

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

		MacFromUint64_t(packet->site, site_address);

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

		MacFromUint64_t(packet->site, site_address);

		memcpy(ptr, reinterpret_cast<char*>(packet), packet->size);
	}

	void LIFXController::SetPowerPacket(uint64_t site_address, uint64_t mac, uint8_t seq, uint16_t power, void* ptr)
	{
		auto packet = new lifx_header();
		memset(packet, 0, sizeof(lifx_header));
		packet->size = sizeof(lifx_header) + 2;
		packet->type = 21;
		packet->ack_required = 0;
		packet->res_required = 1;
		packet->sequence = seq;
		packet->source = 666;
		packet->protocol = _byteswap_ushort(0x34);

		MacFromUint64_t(packet->site, site_address);

		char buffer[38];
		ZeroMemory(buffer, sizeof(buffer));
		memcpy(buffer, packet, 36);

		Uint16_tToCharArray(power, &buffer[36]);

		memcpy(ptr, reinterpret_cast<char*>(buffer), sizeof(buffer));
	}

	void LIFXController::SetLabelPacket(uint64_t site_address, uint64_t mac, uint8_t seq, const void* label, uint8_t size, void* ptr)
	{
		auto packet = new lifx_header();
		memset(packet, 0, sizeof(lifx_header));
		packet->size = sizeof(lifx_header) + 32;
		packet->type = 24;
		packet->ack_required = 0;
		packet->res_required = 1;
		packet->sequence = seq;
		packet->source = 666;
		packet->protocol = _byteswap_ushort(0x34);

		MacFromUint64_t(packet->site, site_address);

		char buffer[68];
		ZeroMemory(buffer, sizeof(buffer));

		memcpy(buffer, packet, 36);
		memcpy(&buffer[36], label, size);

		memcpy(ptr, reinterpret_cast<char*>(buffer), sizeof(buffer));
	}

	void LIFXController::SetLightColorPacket(
		uint64_t site_address,
		uint64_t mac,
		uint8_t seq,
		uint16_t h,
		uint16_t s,
		uint16_t b,
		uint16_t k,
		uint32_t d,
		void* ptr)
	{
		auto packet = new lifx_header();
		memset(packet, 0, sizeof(lifx_header));
		packet->size = sizeof(lifx_header) + 13;
		packet->type = 102;
		packet->ack_required = 0;
		packet->res_required = 1;
		packet->sequence = seq;
		packet->source = 666;
		packet->protocol = _byteswap_ushort(0x34);

		MacFromUint64_t(packet->site, site_address);

		char buffer[49];
		ZeroMemory(buffer, sizeof(buffer));

		memcpy(buffer, packet, 36);
		Uint16_tToCharArray(h, &buffer[37]);
		Uint16_tToCharArray(s, &buffer[39]);
		Uint16_tToCharArray(b, &buffer[41]);
		Uint16_tToCharArray(k, &buffer[43]);
		
		buffer[48] = d;
		buffer[47] = d >> 8;
		buffer[46] = d >> 16;
		buffer[45] = d >> 24;

		memcpy(ptr, reinterpret_cast<char*>(buffer), sizeof(buffer));
	}

	LIFXController::~LIFXController()
	{}

	void LIFXController::MacFromUint64_t(uint8_t* target, uint64_t value)
	{
		// ignoring two zero bytes at the end because of Converter.ToInt64()
		// that pads unnecesarily
		target[0] = uint8_t((value >> 56) & 0xFF);
		target[1] = uint8_t((value >> 48) & 0xFF);
		target[2] = uint8_t((value >> 40) & 0xFF);
		target[3] = uint8_t((value >> 32) & 0xFF);
		target[4] = uint8_t((value >> 24) & 0xFF);
		target[5] = uint8_t((value >> 16) & 0xFF);
	}

	void LIFXController::Uint16_tToCharArray(uint16_t payload, char* target)
	{
		target[1] = payload >> 8;
		target[0] = payload & 0xFF;
	}
}
