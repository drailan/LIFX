#include "Lifx.h"

using namespace std;

#pragma comment(lib, "Ws2_32.lib")
#pragma comment(lib, "iphlpapi.lib")

namespace LIFX
{
	LIFXController* LIFXEntry::instance;

	LIFXController* LIFXEntry::getInstance()
	{
		if (instance == nullptr)
		{
			instance = new LIFXController();
		}

		return instance;
	}

	void LIFXEntry::Discover() { getInstance()->Discover(); }
	void LIFXEntry::SetPower(wstring s, uint16_t p)	{ getInstance()->SetPower(s.c_str(), p); }

	void LIFXEntry::SetLightColor(wstring s, uint16_t* p)
	{
		getInstance()->SetLightColor(s.c_str(), p);
	}

	bool LIFXEntry::PopulateLabels(BSTR* names)
	{
		auto labels = getInstance()->GetLabels();

		if (labels.size() == 0) return FALSE;

		for (auto i = 0; i < labels.size(); ++i) {
			names[i] = SysAllocString(labels.at(i).c_str());
		}
		return TRUE;
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
