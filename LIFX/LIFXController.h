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
	} lifx_header;
#pragma pack(pop)

	class LIFXController
	{
	public:
		LIFXController();
		~LIFXController();


		void GetDiscoveryPacket(uint8_t, void*);
		void GetLabelPacket(uint64_t, uint8_t, void*);
		void GetLightStatePacket(uint64_t, uint8_t, void*);
		void GetGroupPacket(uint64_t, uint8_t, void*);

		void SetPowerPacket(uint64_t, uint64_t, uint8_t, uint16_t, void*);
		void SetLabelPacket(uint64_t, uint64_t, uint8_t, const void*, uint8_t, void*);
		void SetLightColorPacket(uint64_t, uint64_t, uint8_t, uint16_t, uint16_t, uint16_t, uint16_t, uint32_t, void*);

	private:
		static void MacFromUint64_t(uint8_t*, uint64_t);
		static void Uint16_tToCharArray(uint16_t, char*);
	};
}
