using LIFXSeeSharp.Bulb;

namespace LIFXSeeSharp.Packet
{
	interface IPacket
	{
		byte Sequence { get; }
		void ProcessBulb(IBulb bulb);
	}
}
