using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace LifxSeeSharp.Packet
{
	public class DiscoveryPacket : BasePacket
	{
		public override byte PacketType { get { return 0x003; } }
		public uint Port { get; private set; }

		public DiscoveryPacket(byte[] payload, IPAddress ip) : base(payload, ip)
		{
		}

		public override void ProcessPayload()
		{
			base.ProcessPayload();

			var portSubArray = new byte[4];
			Array.Copy(Payload, 37, portSubArray, 0, 4);
			Port = (uint)(BitConverter.ToUInt32(portSubArray, 0));
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			return sb.AppendFormat(CultureInfo.CurrentCulture, "Discovery packet: IP {0}, Port {1}, Sequence {2}", IP.ToString(), Port, Sequence).ToString();
		}
	}
}
