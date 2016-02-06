using System;
using System.Net;
using System.Text;

namespace LIFXSeeSharp.Packet
{
	class DiscoveryPacket : BasePacket
	{
		public override byte Type { get { return 0x003; } }
		public uint Port { get; private set; }

		public DiscoveryPacket() : base()
		{
		}

		public DiscoveryPacket(byte[] payload, IPAddress ip) : base(payload, ip)
		{
		}

		protected override void ProcessPayload()
		{
			base.ProcessPayload();

			var portSubArray = new byte[4];
			Array.Copy(Payload, 37, portSubArray, 0, 4);
			Port = (uint)(BitConverter.ToUInt32(portSubArray, 0));
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			return sb.AppendFormat("Discovery packet: IP {0}, Port {1}, Sequence {2}", IP.ToString(), Port, Sequence).ToString();
		}
	}
}
