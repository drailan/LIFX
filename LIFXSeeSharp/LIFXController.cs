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
using System.Threading;

// Hue => 0,360
// Saturation => %
// Brightness => %
// Kelvin => 2500,9000
// Dim => ms
// Power => 0 & 65535

namespace LifxSeeSharp
{
	public sealed class LifxController : IDisposable
	{
		private readonly string TAG = "LIFXController";

		public List<LifxBulb> Bulbs { get; private set; }

		private NetworkManager _networkManager;

		private Subject<LifxBulb> _discoverySubject;
		private Subject<LifxBulb> _labelSubject;
		private Subject<LifxBulb> _groupSubject;

		private List<KeyValuePair<byte, LifxBulb>> _sentPackets;

		CancellationToken _ct;
		CancellationTokenSource _cts;

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

		public IObservable<LifxBulb> ObserveBulbDiscovery()
		{
			return _discoverySubject;
		}

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

		public void SetLabel(IBulb bulb, string label)
		{
			var b = bulb as LifxBulb;

			if (b != null)
			{
				var data = new byte[PacketSize.SET_LABEL];
				var seq = SequenceGenerator.Next;
				NativeMethods.SetLabelPacket(b.SiteAddress, b.Mac, seq, label, data);

				_sentPackets.Add(new KeyValuePair<byte, LifxBulb>(seq, b));
				_networkManager.SendTargetedPacket(data, seq, b.IP);
			}
		}

		public void GetLightState(IBulb bulb)
		{
			var b = bulb as LifxBulb;
			if (b != null)
			{
				var data = new byte[PacketSize.GET_LIGHT_STATE];
				var seq = SequenceGenerator.Next;
				NativeMethods.GetLightStatePacket(b.SiteAddress, seq, data);

				_sentPackets.Add(new KeyValuePair<byte, LifxBulb>(seq, b));
				_networkManager.SendTargetedPacket(data, seq, b.IP);
			}
		}

		public void SetPower(IBulb bulb, ushort power)
		{
			var data = new byte[PacketSize.SET_POWER];
			var seq = SequenceGenerator.Next;

			var b = bulb as LifxBulb;
			if (b != null)
			{
				NativeMethods.SetPowerPacket(b.SiteAddress, b.Mac, seq, power, data);

				_sentPackets.Add(new KeyValuePair<byte, LifxBulb>(seq, b));
				_networkManager.SendTargetedPacket(data, seq, b.IP);
			}
		}

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