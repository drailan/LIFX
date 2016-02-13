using LIFXSeeSharp.Bulb;

namespace LIFXSeeSharp.Packet
{
	public interface IPacket
	{
		byte Sequence { get; }
		void ProcessBulb(IBulb bulb);
	}
}
