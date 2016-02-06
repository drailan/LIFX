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

	void LIFXEntry::SetPower(wstring s, uint16_t p)
	{
		getInstance()->SetPower(s.c_str(), p);
	}

	void LIFXEntry::GetPower(wstring s, uint16_t* state)
	{
		auto power_state = getInstance()->GetPower(s.c_str());
		state[0] = power_state;
	}

	void LIFXEntry::SetLightColor(wstring s, uint16_t* p)
	{
		getInstance()->SetLightColor(s.c_str(), p);
	}

	bool LIFXEntry::GetLightState(wstring s, uint32_t* p)
	{
		auto light_state = getInstance()->GetLightState(s.c_str());
		p[0] = light_state.hue;
		p[1] = light_state.saturation;
		p[2] = light_state.brightness;
		p[3] = light_state.kelvin;
		p[4] = light_state.dim;
		p[5] = light_state.power;
		return TRUE;
	}
}
