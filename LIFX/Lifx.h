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
			static bool PopulateLabels(wchar_t**);
			static bool PopulateGroups(wchar_t**);
			static bool GetLightState(std::wstring, uint32_t*);
			static void SetPower(std::wstring, uint16_t);
            static void GetPower(std::wstring, uint16_t*);
			static void SetLightColor(std::wstring, uint16_t*);
		private:
			static LIFXController* getInstance(void);
			static LIFXController* instance;
	};

	extern "C" { LIFX_API inline void Discover() { LIFXEntry::Discover(); }}
	extern "C" { LIFX_API inline void GetLabels(wchar_t** s) { LIFXEntry::PopulateLabels(s); } }
	extern "C" { LIFX_API inline void GetGroups(wchar_t** s) { LIFXEntry::PopulateGroups(s); } }
	extern "C" { LIFX_API inline void SetPower(const wchar_t* s, uint16_t onoff)
	{
		assert(s != nullptr);
		std::wstring ws(s, wcslen(s));
		LIFXEntry::SetPower(ws, onoff);
	}}

    extern "C" {LIFX_API inline void GetPower(const wchar_t* s, uint16_t* powerstate)
    {
        assert(s != nullptr);
        std::wstring ws(s, wcslen(s));
        LIFXEntry::GetPower(ws, powerstate);
    }}

	extern "C" { LIFX_API inline void GetLightState(const wchar_t* target, uint32_t* state)
	{
		assert(target != nullptr);
		std::wstring ws(target, wcslen(target));
		LIFXEntry::GetLightState(ws, state);
	}}

	extern "C" { LIFX_API inline void SetLightColor(const wchar_t* target, uint16_t* state)
	{
		assert(target != nullptr);
		std::wstring ws(target, wcslen(target));
		LIFXEntry::SetLightColor(ws, state);
	}}
}
