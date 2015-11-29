#pragma once

#include <stdint.h>
#include <winsock2.h>
#include <vector>

#define DEFAULT_PORT 56700
#define HEADER_SIZE 36
#define NUM_LAMPS 4

namespace LIFX
{

#pragma pack(push, 1)
	typedef struct
	{
		/* frame */
		uint16_t size; // LE
		uint16_t protocol : 16;
		uint32_t source; // reserved

		 /* frame address */
		uint8_t  target[6];
		uint16_t: 16; // always 0 ( mac padding )
		uint8_t site[6];
		uint16_t:16; // reserved 3

		/* protocol header */
		uint64_t:64; // timestamp
		uint16_t type;
		uint16_t:16;
		/* variable length payload follows */
	} lifx_header;
#pragma pack(pop)

	typedef struct
	{
		wchar_t label[256];
		char ip[50];
		uint64_t mac;
		uint64_t site_address;
	} lifx_bulb;

	typedef struct
	{
		uint16_t hue;
		uint16_t saturation;
		uint16_t brightness;
		uint16_t kelvin;
		uint16_t dim;
		uint16_t power;
		char label[32];
	} light_state;

	class LIFXController
	{
	public:
		LIFXController();
		~LIFXController();
		
		void Discover();
		std::vector<std::wstring> GetLabels();

		void SetPower(const wchar_t*, uint16_t);
		void SetLightColor(const wchar_t*, uint16_t*);
		light_state GetLightState(const wchar_t*);
	private:
		void init_network();
		void get_bulb_info();
		std::wstring get_label(const lifx_bulb);
		static uint32_t InvertAndConvertHexBufToUint(char[2]);

		static uint64_t string_to_mac(const char*);
		static uint64_t get_mac(char*);

		static void print_hex_memory(void*);

		SOCKET bcast_socket;
		SOCKET out_socket;
		SOCKET in_socket;
		WSADATA wsaData;

		int count;
		char* site_address = "4c:49:46:58:56:32";
		sockaddr_in addr;

		std::vector<lifx_bulb> bulbs;
	};
}
