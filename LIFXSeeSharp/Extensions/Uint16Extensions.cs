using LIFXSeeSharp.Packet;
using System;
using System.Net;

namespace LIFXSeeSharp.Extensions
{
	static class Uint16Extensions
	{
		public static Func<byte[], IPAddress, BasePacket> ToPacket(this ushort value)
		{
			Func<byte[], IPAddress, BasePacket> creator;
			switch (value)
			{
				case 0x003:
					creator = (payload, ip) => { return new DiscoveryPacket(payload, ip); };
					break;
				case 0x019:
					creator = (payload, ip) => { return new LabelPacket(payload, ip); };
					break;
				default:
					creator = (payload, ip) => { return new BasePacket(payload, ip); };
					break;
			}

			return creator;
		}
	}
}
