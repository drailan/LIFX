#include "Lifx.h"

using namespace std;

#pragma comment(lib, "Ws2_32.lib")
#pragma comment(lib, "iphlpapi.lib")

namespace LIFX
{
	LIFXController* LIFXEntry::instance;

	LIFXController* LIFXEntry::getInstance()
	{
		if (instance == nullptr) {
			instance = new LIFXController();
		}

		return instance;
	}

	void LIFXEntry::GetDiscoveryPacket(uint8_t s, void* ptr)
	{
		getInstance()->GetDiscoveryPacket(s, ptr);
	}

	void LIFXEntry::GetLabelPacket(uint64_t site, uint8_t seq, void* ptr)
	{
		getInstance()->GetLabelPacket(site, seq, ptr);
	}

	void LIFXEntry::GetLightStatePacket(uint64_t site, uint8_t seq, void* ptr)
	{
		getInstance()->GetLightStatePacket(site, seq, ptr);
	}

	void LIFXEntry::GetGroupPacket(uint64_t site, uint8_t seq, void* ptr)
	{
		getInstance()->GetGroupPacket(site, seq, ptr);
	}

	void LIFXEntry::SetPowerPacket(uint64_t site, uint64_t mac, uint8_t seq, uint16_t power, void* ptr)
	{
		getInstance()->SetPowerPacket(site, mac, seq, power, ptr);
	}

	void LIFXEntry::SetLabelPacket(uint64_t site, uint64_t mac, uint8_t seq, void* label, uint8_t size, void* ptr)
	{
		getInstance()->SetLabelPacket(site, mac, seq, label, size, ptr);
	}

	void LIFXEntry::SetLightColorPacket(uint64_t site, uint64_t mac, uint8_t seq, uint16_t h, uint16_t s, uint16_t b, uint16_t k, uint32_t d, void* ptr)
	{
		getInstance()->SetLightColorPacket(site, mac, seq, h, s, b, k, d, ptr);
	}
}
