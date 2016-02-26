using LifxSeeSharp.Bulb;
using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace LifxSeeSharp.Packet
{
	internal class GroupPacket : BasePacket
	{
		private readonly string TAG = "LabelPacket";

		public override byte PacketType { get { return 0x0035; } }

		public string Group { get; private set; }

		public ulong UpdatedAt { get; private set; }

		public byte[] GroupBytes { get; private set; }

		public GroupPacket(byte[] payload, IPAddress ip) : base(payload, ip)
		{
		}

		public override void ProcessPayload()
		{
			base.ProcessPayload();

			var groupSubArray = new byte[32];
			var updatedAtSubAray = new byte[8];

			GroupBytes = new byte[16];

			Array.Copy(Payload, 36, GroupBytes, 0, 16);
			Array.Copy(Payload, 52, groupSubArray, 0, 32);
			Array.Copy(Payload, 84, updatedAtSubAray, 0, 8);

			Group = Encoding.UTF8.GetString(groupSubArray).TrimEnd('\0');
			UpdatedAt = BitConverter.ToUInt64(updatedAtSubAray, 0);
		}

		public override void ProcessBulb(IBulb bulb)
		{
			Log.Debug(TAG, "Processing bulb group");
			var b = bulb as LifxBulb;
			if (b != null)
			{
				b.Group = Group;
			}
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			return sb.AppendFormat(CultureInfo.CurrentCulture, "Label packet: IP {0}, Group {1}, Updated at {2} Sequence {3}",
				IP.ToString(), UpdatedAt, Group, Sequence).ToString();
		}
	}
}
