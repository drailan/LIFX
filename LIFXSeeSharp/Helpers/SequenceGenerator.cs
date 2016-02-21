using System;

namespace LifxSeeSharp.Helpers
{
	public static class SequenceGenerator
	{
		private const byte _max = Byte.MaxValue;
		private static byte _current = 1;

		public static byte Current { get { return _current; } }

		public static byte Next
		{
			get { return _current == _max ? Reset() : ++_current; }
		}

		private static byte Reset()
		{
			return _current = 1;
		}
	}
}
