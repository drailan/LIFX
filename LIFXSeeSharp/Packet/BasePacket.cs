using LifxSeeSharp.Bulb;
using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace LifxSeeSharp.Packet
{
	public class BasePacket : IPacket
	{
		public virtual byte PacketType { get { return 0x0; } }
		public IPAddress IP { get; private set; }
		public ulong Mac { get; private set; }
		public ulong Site { get; private set; }
		public byte[] Payload { get; private set; }
		public byte Sequence { get; private set; }

		public BasePacket(byte[] payload, IPAddress ip)
		{
			Payload = payload;
			IP = ip;
		}

		public virtual void ProcessPayload()
		{
			var macSubArray = new byte[8];
			var siteAddressSubArray = new byte[8];
			var sequenceSubArray = new byte[1];

			Array.Copy(Payload, 8, macSubArray, 0, 6);
			Array.Copy(Payload, 16, siteAddressSubArray, 0, 6);
			Array.Copy(Payload, 23, sequenceSubArray, 0, 1);

			Mac = (ulong)IPAddress.NetworkToHostOrder(BitConverter.ToInt64(macSubArray, 0));
			Site = (ulong)IPAddress.NetworkToHostOrder(BitConverter.ToInt64(siteAddressSubArray, 0));
			Sequence = sequenceSubArray[0];
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			return sb.AppendFormat(CultureInfo.CurrentCulture, "Base packet: IP {0}, Sequence {1}", IP.ToString(), Sequence).ToString();
		}

		public virtual void ProcessBulb(IBulb bulb)
		{
		}
	}
}
