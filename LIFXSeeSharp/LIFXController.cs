using LifxSeeSharp.Bulb;
using LifxSeeSharp.Helpers;
using LifxSeeSharp.Native;
using LifxSeeSharp.Network;
using LifxSeeSharp.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;

// Hue => 0,360
// Saturation => %
// Brightness => %
// Kelvin => 2500,9000
// Dim => ms
// Power => 0 & 65535

namespace LifxSeeSharp
{
	/// <summary>
	/// Controller class, library entry point
	/// </summary>
	public sealed class LifxController : IDisposable
	{
		private readonly string TAG = "LIFXController";

		/// <summary>
		/// Contains the bulb collection
		/// </summary>
		public List<LifxBulb> Bulbs { get; private set; }

		private NetworkManager _networkManager;

		private Subject<LifxBulb> _discoverySubject;
		private Subject<LifxBulb> _labelSubject;
		private Subject<LifxBulb> _groupSubject;

		private List<KeyValuePair<byte, LifxBulb>> _sentPackets;

		CancellationToken _ct;
		CancellationTokenSource _cts;

		/// <summary>
		/// Initialize the Lifx controller, reponsible for working with light bulbs
		/// </summary>
		public LifxController()
		{
			_networkManager = new NetworkManager();
			_discoverySubject = new Subject<LifxBulb>();
			_labelSubject = new Subject<LifxBulb>();
			_groupSubject = new Subject<LifxBulb>();
			_sentPackets = new List<KeyValuePair<byte, LifxBulb>>();

			_cts = new CancellationTokenSource();
			_ct = _cts.Token;

			InitObservableProperties();
		}

		private void InitObservableProperties()
		{
			_networkManager.UdpListener()
				.ObserveOn(NewThreadScheduler.Default)
				.Do(p => Log.Debug(TAG, "Received: {0}", p))
				.Where(packet => packet.Sequence != 0)
				.Do(packet =>
				{
					var discoveryPacket = packet as DiscoveryPacket;
					if (discoveryPacket != null)
					{
						var duplicate = Bulbs.Any(b => b.IP.Equals(discoveryPacket.IP));

						if (!duplicate)
						{
							var bulb = new LifxBulb()
							{
								Mac = discoveryPacket.Mac,
								IP = discoveryPacket.IP,
								SiteAddress = discoveryPacket.Site
							};

							Bulbs.Add(bulb);
							_discoverySubject.OnNext(bulb);
							_labelSubject.OnNext(bulb);
							_groupSubject.OnNext(bulb);
						}
					}
					else
					{
						var sentPacket = _sentPackets.Where(p => p.Key == packet.Sequence).FirstOrDefault();
						packet.ProcessBulb(sentPacket.Value);
						_sentPackets.Remove(sentPacket);
					}
				})
				.Subscribe(_ct);

			_labelSubject.Subscribe(bulb =>
				{
					Thread.Sleep(750); // don't want to spam packets, bulbs might ignore some

					var data = new byte[PacketSize.GET_LABEL];
					var seq = SequenceGenerator.Next;
					NativeMethods.GetLabelPacket(bulb.SiteAddress, seq, data);

					_sentPackets.Add(new KeyValuePair<byte, LifxBulb>(seq, bulb));
					_networkManager.SendTargetedPacket(data, seq, bulb.IP);
				},
				_ct);

			_groupSubject.Subscribe(bulb =>
			{
				Thread.Sleep(750);
				var data = new Byte[PacketSize.GET_GROUP];
				var seq = SequenceGenerator.Next;
				NativeMethods.GetGroupPacket(bulb.SiteAddress, seq, data);

				_sentPackets.Add(new KeyValuePair<byte, LifxBulb>(seq, bulb));
				_networkManager.SendTargetedPacket(data, seq, bulb.IP);
			},
			_ct);
		}

		/// <summary>
		/// Subscribe to bulb discovery subject
		/// </summary>
		/// <returns></returns>
		public IObservable<LifxBulb> ObserveBulbDiscovery()
		{
			return _discoverySubject;
		}

		/// <summary>
		/// Discover light bulbs on the lan
		/// </summary>
		public void RunInitialDiscovery()
		{
			if (Bulbs == null)
			{
				Bulbs = new List<LifxBulb>();
			}

			Bulbs.Clear();

			var data = new byte[PacketSize.DISCOVERY];
			var seq = SequenceGenerator.Next;
			NativeMethods.GetDiscoveryPacket(seq, data);

			_networkManager.Discover(data, seq);
		}

		/// <summary>
		/// Query every bulb for its light state
		/// </summary>
		public void GetLightStates()
		{
			Bulbs.ForEach(b =>
			{
				Thread.Sleep(750); // don't want to spam packets, bulbs might ignore some

				var data = new byte[PacketSize.GET_LIGHT_STATE];
				var seq = SequenceGenerator.Next;
				NativeMethods.GetLightStatePacket(b.SiteAddress, seq, data);

				_sentPackets.Add(new KeyValuePair<byte, LifxBulb>(seq, b));
				_networkManager.SendTargetedPacket(data, seq, b.IP);
			});
		}

		/// <summary>
		/// Set the label for the target bulb
		/// </summary>
		/// <param name="target">Target bulb</param>
		/// <param name="label">label string, will be truncated to 32 bytes</param>
		public void SetLabel(IBulb target, string label)
		{
			var b = target as LifxBulb;

			if (b != null)
			{
				var labelBytes = Encoding.UTF8.GetBytes(label);

				var data = new byte[PacketSize.SET_LABEL];
				var seq = SequenceGenerator.Next;
				NativeMethods.SetLabelPacket(
					b.SiteAddress,
					b.Mac,
					seq,
					labelBytes,
					Math.Min((uint)labelBytes.Length, 32),
					data);

				_sentPackets.Add(new KeyValuePair<byte, LifxBulb>(seq, b));
				_networkManager.SendTargetedPacket(data, seq, b.IP);
			}
		}

		/// <summary>
		/// Request the light state for the target bulb
		/// </summary>
		/// <param name="target">Target bulb</param>
		public void GetLightState(IBulb target)
		{
			var b = target as LifxBulb;
			if (b != null)
			{
				var data = new byte[PacketSize.GET_LIGHT_STATE];
				var seq = SequenceGenerator.Next;
				NativeMethods.GetLightStatePacket(b.SiteAddress, seq, data);

				_sentPackets.Add(new KeyValuePair<byte, LifxBulb>(seq, b));
				_networkManager.SendTargetedPacket(data, seq, b.IP);
			}
		}

		/// <summary>
		/// Set the light state fo the bulb
		/// </summary>
		/// <param name="target">Target bulb</param>
		/// <param name="h">Hue, range from 0 to 360</param>
		/// <param name="s">Saturation, 0 to 100</param>
		/// <param name="b">Brightness, 0 to 100</param>
		/// <param name="k">Kelvin, 2500 to 9000</param>
		/// <param name="d">Dim, color transition in milliseconds</param>
		public void SetLightState(IBulb target, ushort h, ushort s, ushort b, ushort k, uint d)
		{
			var bulb = target as LifxBulb;
			if (bulb != null)
			{
				var data = new byte[PacketSize.SET_LIGHT_STATE];
				var seq = SequenceGenerator.Next;
				NativeMethods.SetLightColorPacket(bulb.SiteAddress, bulb.Mac, seq, h, s, b, k, d, data);

				_sentPackets.Add(new KeyValuePair<byte, LifxBulb>(seq, bulb));
				_networkManager.SendTargetedPacket(data, seq, bulb.IP);
			}
		}

		/// <summary>
		/// Set the bulb power
		/// </summary>
		/// <param name="target">The target bulb</param>
		/// <param name="power">Power, 0 off, 0xFFFF on</param>
		public void SetPower(IBulb target, ushort power)
		{
			var data = new byte[PacketSize.SET_POWER];
			var seq = SequenceGenerator.Next;

			var b = target as LifxBulb;
			if (b != null)
			{
				NativeMethods.SetPowerPacket(b.SiteAddress, b.Mac, seq, power, data);

				_sentPackets.Add(new KeyValuePair<byte, LifxBulb>(seq, b));
				_networkManager.SendTargetedPacket(data, seq, b.IP);
			}
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			if (Bulbs != null)
			{
				Bulbs.Clear();
			}

			_cts.Cancel();
			_cts.Dispose();

			_groupSubject.Dispose();
			_discoverySubject.Dispose();
			_labelSubject.Dispose();

			_networkManager.Dispose();
		}
	}
}