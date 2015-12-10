using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace LIFXSeeSharp
{
	internal static class Program
	{
        private static void Main(string[] args)
		{
            var c = new LifxController();
            c.RunInitialDiscovery();
            c.GetLightState();

            c.SetPower(1, "La");
            c.SetLightState(187, 0.84f, 0.15f, 3500, 500, "La");

            Console.WriteLine("Done");
			Console.ReadKey();
		}
	}
}
