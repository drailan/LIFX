using System;
using System.Text;

namespace LIFXSeeSharp
{
	public class LifxBulb
	{
		public string Label { get; set; }
		public float Hue { get; set;  }
		public float Saturation { get; set;  }
		public float Brightness { get; set; }
		public ushort Kelvin { get; set; }
		public ushort Dim { get; set; }
		public ushort Power { get; set;  }


		public LifxBulb(string label = "")
		{
			Label = label;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("Label: {0}\n\tHue: {1}\n\tSaturation: {2}\n\tBrightNess: {3}\n\tKelvin: {4}\n\tDim: {5}\n\tPower: {6}\n",
				Label, Hue, Saturation, Brightness, Kelvin, Dim, Power);
			return sb.ToString();
		}
	}
}