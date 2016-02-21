#ifdef LIFX_API_EXPORTS
#define LIFX_API __declspec(dllexport) 
#else
#define LIFX_API __declspec(dllimport) 
#endif

#include "LIFXController.h"
#include <cassert>

namespace LIFX
{
	class LIFXEntry
	{
	public:
		static void GetDiscoveryPacket(uint8_t, void*);
		static void GetLabelPacket(uint64_t, uint8_t, void*);
		static void GetLightStatePacket(uint64_t, uint8_t, void*);
		static void GetGroupPacket(uint64_t, uint8_t, void*);

		static void SetPowerPacket(uint64_t, uint64_t, uint8_t, uint16_t, void*);

	private:
		static LIFXController* getInstance(void);
		static LIFXController* instance;
	};

	extern "C" { LIFX_API inline void GetDiscoveryPacket(uint8_t s, void* ptr) { LIFXEntry::GetDiscoveryPacket(s, ptr); }}
	extern "C" { LIFX_API inline void GetLabelPacket(uint64_t s, uint8_t seq, void* ptr) { LIFXEntry::GetLabelPacket(s, seq, ptr); }}
	extern "C" { LIFX_API inline void GetLightStatePacket(uint64_t s, uint8_t seq, void* ptr) { LIFXEntry::GetLightStatePacket(s, seq, ptr); }}
	extern "C" { LIFX_API inline void GetGroupPacket(uint64_t s, uint8_t seq, void* ptr) { LIFXEntry::GetGroupPacket(s, seq, ptr); }}
	extern "C" { LIFX_API inline void SetPowerPacket(uint64_t s, uint64_t mac, uint8_t seq, uint16_t p, void* ptr) { LIFXEntry::SetPowerPacket(s, mac, seq, p, ptr); }}
}
