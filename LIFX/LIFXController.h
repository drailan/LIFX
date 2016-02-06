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

		uint32_t source; // setting source to anything other then 0 will make the bulbs send a unicast

		 /* frame address */
		uint8_t target[6];
		uint8_t padding[2]; // always 0 ( 2 byte mac padding )
		uint8_t site[6];

		uint8_t  res_required : 1;
		uint8_t  ack_required : 1;
	uint8_t:6;
		uint8_t  sequence;

		/* protocol header */
		uint64_t timestamp : 64; // timestamp
		uint16_t type;
	uint16_t:16;
		/* variable length payload follows */
	} lifx_header;
#pragma pack(pop)

	typedef struct
	{
		wchar_t group[256];
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


		void GetDiscoveryPacket(uint8_t, void*);
		void GetLabelPacket(uint64_t, uint8_t, void*);
		void GetLightStatePacket(uint64_t, uint8_t, void*);

		void SetPower(const wchar_t*, uint16_t);
		uint16_t GetPower(const wchar_t*);


		void SetLightColor(const wchar_t*, uint16_t*);
		light_state GetLightState(const wchar_t*);

	private:
		std::wstring GetLabel(const lifx_bulb);
		std::wstring GetGroup(const lifx_bulb);
		static uint32_t InvertAndConvertHexBufToUint(char[2]);

		static void FormMac(uint8_t*, uint64_t);
		static uint64_t StringToMac(const char*);
		static uint64_t* StringToMacPtr(const char*);
		static uint64_t GetMacFromIP(char*);

		SOCKET bcast_socket;
		SOCKET out_socket;
		SOCKET in_socket;
		WSADATA wsaData;
		sockaddr_in addr;
		std::vector<lifx_bulb> bulbs;
	};
}
