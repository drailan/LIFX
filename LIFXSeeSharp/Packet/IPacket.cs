using LifxSeeSharp.Bulb;

namespace LifxSeeSharp.Packet
{
	internal interface IPacket
	{
		byte Sequence { get; }
		void ProcessBulb(IBulb bulb);
		void ProcessPayload();
	}
}
