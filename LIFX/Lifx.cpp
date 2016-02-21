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

	void LIFXEntry::GetLabelPacket(uint64_t s, uint8_t seq, void* ptr)
	{
		getInstance()->GetLabelPacket(s, seq, ptr);
	}

	void LIFXEntry::GetLightStatePacket(uint64_t s, uint8_t seq, void* ptr)
	{
		getInstance()->GetLightStatePacket(s, seq, ptr);
	}

	void LIFXEntry::GetGroupPacket(uint64_t s, uint8_t seq, void* ptr)
	{
		getInstance()->GetGroupPacket(s, seq, ptr);
	}

	void LIFXEntry::SetPowerPacket(uint64_t s, uint64_t mac, uint8_t seq, uint16_t power, void* ptr)
	{
		getInstance()->SetPowerPacket(s, mac, seq, power, ptr);
	}
	void LIFXEntry::SetLabelPacket(uint64_t s, uint64_t mac, uint8_t seq, char* label, void* ptr)
	{
		getInstance()->SetLabelPacket(s, mac, seq, label, ptr);
	}
}
