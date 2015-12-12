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
			static void Discover();
			static bool PopulateLabels(BSTR*);
			static bool PopulateGroups(BSTR*);
			static bool GetLightState(std::wstring, uint32_t*);
			static void SetPower(std::wstring, uint16_t);
			static void SetLightColor(std::wstring, uint16_t*);
		private:
			static LIFXController* getInstance(void);
			static LIFXController* instance;
	};

	extern "C" { LIFX_API inline void Discover() { LIFXEntry::Discover(); }}
	extern "C" { LIFX_API inline void GetLabels(BSTR* s) { LIFXEntry::PopulateLabels(s); } }
	extern "C" { LIFX_API inline void GetGroups(BSTR* s) { LIFXEntry::PopulateGroups(s); } }
	extern "C" { LIFX_API inline void SetPower(BSTR s, uint16_t onoff)
	{
		assert(s != nullptr);
		std::wstring ws(s, wcslen(s));
		LIFXEntry::SetPower(ws, onoff);
	}}

	extern "C" { LIFX_API inline void GetLightState(BSTR target, uint32_t* state)
	{
		assert(target != nullptr);
		std::wstring ws(target, wcslen(target));
		LIFXEntry::GetLightState(ws, state);
	}}

	extern "C" { LIFX_API inline void SetLightColor(BSTR target, uint16_t* state)
	{
		assert(target != nullptr);
		std::wstring ws(target, wcslen(target));
		LIFXEntry::SetLightColor(ws, state);
	}}
}
