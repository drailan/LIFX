using LifxSeeSharp.Bulb;
using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace LifxSeeSharp.Packet
{
	public class LightStatePacket : BasePacket
	{
		private readonly string TAG = "LightStatePacket";

		public override byte PacketType { get { return 0x006b; } }

		public float Hue { get; private set; }

		public float Saturation { get; private set; }

		public float Brightness { get; private set; }

		public ushort Kelvin { get; private set; }

		public ushort Dim { get; private set; }

		public ushort Power { get; private set; }

		public LightStatePacket(byte[] payload, IPAddress ip) : base(payload, ip)
		{
		}

		public override void ProcessPayload()
		{
			base.ProcessPayload();

			var hueSubArray = new byte[2];
			var saturationSubArray = new byte[2];
			var brightnessSubArray = new byte[2];
			var kelvinSubArray = new byte[2];
			var dimSubArray = new byte[2];
			var powerSubArray = new byte[2];

			Array.Copy(Payload, 36, hueSubArray, 0, 2);
			Array.Copy(Payload, 38, saturationSubArray, 0, 2);
			Array.Copy(Payload, 40, brightnessSubArray, 0, 2);
			Array.Copy(Payload, 42, kelvinSubArray, 0, 2);
			Array.Copy(Payload, 44, dimSubArray, 0, 2);
			Array.Copy(Payload, 46, powerSubArray, 0, 2);

			Hue = BitConverter.ToUInt16(hueSubArray, 0);
			Saturation = BitConverter.ToUInt16(saturationSubArray, 0);
			Brightness = BitConverter.ToUInt16(brightnessSubArray, 0);
			Kelvin = BitConverter.ToUInt16(kelvinSubArray, 0);
			Dim = BitConverter.ToUInt16(dimSubArray, 0);
			Power = BitConverter.ToUInt16(powerSubArray, 0);
		}

		public override void ProcessBulb(IBulb bulb)
		{
			Log.Debug(TAG, "Processing bulb light state");
			var b = bulb as LifxBulb;
			if (b != null)
			{
				b.Hue = Hue * 360 / 65535;
				b.Saturation = Saturation / 65535 * 100;
				b.Brightness = Brightness / 65535 * 100;
				b.Kelvin = Kelvin;
				b.Dim = Dim;
				b.Power = Power;
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			return sb.AppendFormat(CultureInfo.CurrentCulture, "Light State packet packet: IP {0}, Sequence {1}", IP.ToString(), Sequence).ToString();
		}
	}
}
