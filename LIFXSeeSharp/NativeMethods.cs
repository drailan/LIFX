using System.Runtime.InteropServices;

namespace LIFXSeeSharp.Native
{
	internal static class NativeMethods
	{
		[DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void GetDiscoveryPacket([In] byte seq, [Out] byte[] packet);

		[DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void GetLabelPacket([In] ulong site, [In] byte seq, [Out] byte[] packet);

		[DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void GetLightStatePacket([In] ulong site, [In] byte seq, [Out] byte[] packet);

		[DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void GetGroupPacket([In] ulong site, [In] byte seq, [Out] byte[] packet);

		[DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern void SetPowerPacket([In] ulong site, [In] ulong mac, [In] byte seq, [In] ushort power, [Out] byte[] packet);
	}
}
