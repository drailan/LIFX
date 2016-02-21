using LifxSeeSharp.Bulb;

namespace LifxSeeSharp.Packet
{
	public interface IPacket
	{
		byte Sequence { get; }
		void ProcessBulb(IBulb bulb);
		void ProcessPayload();
	}
}
