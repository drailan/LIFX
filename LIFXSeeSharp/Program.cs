using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace LIFXSeeSharp
{
	internal static class Program
	{
		static readonly int NUM_BULBS = 4;

		[DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Discover();

		[DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern bool PopulateLabels([Out] IntPtr[] labels);

		[DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern bool SetPower(string label, ushort onoff);

		[DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern bool SetLightColor(string label, ushort[] state);

		[DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern bool GetLightState(string label, [Out] IntPtr[] state);



		private static void Main(string[] args)
		{
			Discover();

			var bulbs = new List<LifxBulb>();
			var labels = new IntPtr[NUM_BULBS];
			PopulateLabels(labels);
			var names = new string[labels.Length];

			for (var i = 0; i < labels.Length; ++i)
			{
				names[i] = Marshal.PtrToStringBSTR(labels[i]);
				Marshal.FreeBSTR(labels[i]);
				bulbs.Add(new LifxBulb(names[i]));
			}

			SetPower("La", 1);

			bulbs.ForEach(bulb =>
			{
				Console.Write(bulb.ToString());
			});

			//bulbs.Where(b => b.Label == "La").ToList().ForEach(b =>
			//{

			//});

			var state = new ushort[5];

			state[0] = Convert.ToUInt16(187 * ushort.MaxValue / 360); // hue * max / 360
			state[1] = Convert.ToUInt16(0.84 * ushort.MaxValue); // %
			state[2] = Convert.ToUInt16(0.15 * ushort.MaxValue); // %
			state[3] = 3500; // between 2500 & 9000
			state[4] = 500;

			bulbs.Where(b => b.Label == "La").ToList().ForEach(b =>
			{
				SetLightColor(b.Label, state);
			});

			//bulbs.ForEach(b =>
			//{
			//	SetLightColor(b.Label, state);
			//});

			Console.ReadKey();
		}
	}
}
