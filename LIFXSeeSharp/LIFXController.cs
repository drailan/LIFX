using LIFXSeeSharp.Bulb;
using LIFXSeeSharp.Helpers;
using LIFXSeeSharp.Logging;
using LIFXSeeSharp.Network;
using LIFXSeeSharp.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading;

// Hue => 0,360
// Saturation => %
// Brightness => %
// Kelvin => 2500,9000
// Dim => ms
// Power => 0 & 65535

namespace LIFXSeeSharp
{
    public class LifxController
    {
        [DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetDiscoveryPacket([In] byte seq, [Out] byte[] packet);

        [DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetLabelPacket([In] ulong site, [In] byte seq, [Out] byte[] packet);

        [DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern bool GetGroups([Out] IntPtr[] labels);

        [DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern bool SetPower(string label, ushort onoff);

        [DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern bool GetPower(string label, [Out] ushort[] state);

        [DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern bool SetLightColor(string label, ushort[] state);

        [DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern bool GetLightState(string label, [Out] uint[] state);

        private readonly string TAG = "LIFXController";

        public List<LifxBulb> Bulbs { get; set; }

        private NetworkManager _networkManager;
        private Subject<LifxBulb> _discoverySubject;
        private Subject<LifxBulb> _labelSubject;

        private List<KeyValuePair<byte, LifxBulb>> _sentPackets;

        public LifxController()
        {
            _networkManager = new NetworkManager();
            _discoverySubject = new Subject<LifxBulb>();
            _labelSubject = new Subject<LifxBulb>();
            _sentPackets = new List<KeyValuePair<byte, LifxBulb>>();

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
                        }
                    }
                    else
                    {
                        var sentPacket = _sentPackets.Where(p => p.Key == packet.Sequence).FirstOrDefault();
                        packet.ProcessBulb(sentPacket.Value);
                        _sentPackets.Remove(sentPacket);
                    }
                })
                .Subscribe();

            _labelSubject.Subscribe(bulb =>
                {
                    Thread.Sleep(750);

                    var data = new byte[PacketSize.LABEL];
                    var seq = SequenceGenerator.GetNext();
                    GetLabelPacket(bulb.SiteAddress, seq, data);

                    _sentPackets.Add(new KeyValuePair<byte, LifxBulb>(seq, bulb));
                    _networkManager.GetLabel(data, seq, bulb.IP);
                });
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
            var seq = SequenceGenerator.GetNext();
            GetDiscoveryPacket(seq, data);

            //_sentPackets.Add(new KeyValuePair<byte,LifxBulb>(seq, null));
            _networkManager.Discover(data, seq);
        }
    }
}