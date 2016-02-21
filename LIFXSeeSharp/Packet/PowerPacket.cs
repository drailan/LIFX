using LIFXSeeSharp.Bulb;
using LIFXSeeSharp.Logging;
using System;
using System.Net;
using System.Text;

namespace LIFXSeeSharp.Packet
{
	class PowerPacket : BasePacket
	{
		private readonly string TAG = "PowerPacket";

		public override byte Type { get { return 0x0016; } }

		public ushort Power { get; private set; }

		public PowerPacket(byte[] payload, IPAddress ip) : base(payload, ip)
		{
		}

		public override void ProcessPayload()
		{
			base.ProcessPayload();

			var powerSubArray = new byte[2];
			Array.Copy(Payload, 36, powerSubArray, 0, 2);
			Power = BitConverter.ToUInt16(powerSubArray, 0);
		}

		public override void ProcessBulb(IBulb bulb)
		{
			Log.Debug(TAG, "Processing bulb power state");
			var b = bulb as LifxBulb;
			if (b != null)
			{
				b.Power = Power;
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			return sb.AppendFormat("Power packet: IP {0}, Sequence {1}, Power {2}", IP.ToString(), Sequence, Power).ToString();
		}
	}
}
