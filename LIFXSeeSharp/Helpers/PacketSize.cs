namespace LifxSeeSharp.Helpers
{
	internal static class PacketSize
	{
		public static readonly int DISCOVERY = 36;
		public static readonly int GET_LABEL = 36;
		public static readonly int GET_LIGHT_STATE = 36;
		public static readonly int GET_GROUP = 36;

		public static readonly int SET_POWER = 38;
		public static readonly int SET_LABEL = 68;
		public static readonly int SET_LIGHT_STATE = 49;
	}
}
