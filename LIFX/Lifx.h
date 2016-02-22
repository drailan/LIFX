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
		static void SetLabelPacket(uint64_t, uint64_t, uint8_t, void*, uint8_t size, void*);
		static void SetLightColorPacket(uint64_t, uint64_t, uint8_t, uint16_t, uint16_t, uint16_t, uint16_t, uint32_t, void*);

	private:
		static LIFXController* getInstance(void);
		static LIFXController* instance;
	};

	extern "C" { LIFX_API inline void GetDiscoveryPacket(uint8_t s, void* ptr) { LIFXEntry::GetDiscoveryPacket(s, ptr); }}
	extern "C" { LIFX_API inline void GetLabelPacket(uint64_t site, uint8_t seq, void* ptr) { LIFXEntry::GetLabelPacket(site, seq, ptr); }}
	extern "C" { LIFX_API inline void GetLightStatePacket(uint64_t site, uint8_t seq, void* ptr) { LIFXEntry::GetLightStatePacket(site, seq, ptr); }}
	extern "C" { LIFX_API inline void GetGroupPacket(uint64_t site, uint8_t seq, void* ptr) { LIFXEntry::GetGroupPacket(site, seq, ptr); }}

	extern "C" { LIFX_API inline void SetPowerPacket(uint64_t site, uint64_t mac, uint8_t seq, uint16_t p, void* ptr) { LIFXEntry::SetPowerPacket(site, mac, seq, p, ptr); }}
	extern "C" { LIFX_API inline void SetLabelPacket(uint64_t site, uint64_t mac, uint8_t seq, void* l, uint8_t size, void* ptr) { LIFXEntry::SetLabelPacket(site, mac, seq, l, size, ptr); }}
	extern "C" { LIFX_API inline void SetLightColorPacket(uint64_t site, uint64_t mac, uint8_t seq, uint16_t h, uint16_t s, uint16_t b, uint16_t k, uint32_t d, void* ptr) { LIFXEntry::SetLightColorPacket(site, mac, seq, h, s, b, k, d, ptr); }}
}
