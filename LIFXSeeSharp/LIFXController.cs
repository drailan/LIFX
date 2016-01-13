using LIFXSeeSharp.Helpers;
using LIFXSeeSharp.Network;
using LIFXSeeSharp.Packet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

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
		private static readonly int NUM_BULBS = 4;

		[DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Discover();

        [DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetDiscoveryPacket([In] byte seq, [Out] byte[] packet);

        [DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetLabelPacket([In] ulong site, [In] byte seq, [Out] byte[] packet);

        [DllImport("LIFX.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		private static extern bool GetLabels([Out] IntPtr[] labels);

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

		public List<LifxBulb> Bulbs { get; set; }

        private NetworkManager _networkManager;
        private Subject<LifxBulb> _discoverySubject;

		public LifxController()
        {
            _networkManager = new NetworkManager();
            _discoverySubject = new Subject<LifxBulb>();

            InitObservableProperties();
        }

        private void InitObservableProperties()
        {
            _networkManager.UdpListener()
                    .ObserveOn(NewThreadScheduler.Default)
                    .Do(packet =>
                    {
                        Console.WriteLine(packet.ToString());

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
                            }
                        }
                        else
                        {
                            var basePacket = packet as BasePacket;
                            var bulb = Bulbs.Where(b => b.IP == basePacket.IP).FirstOrDefault();
                        }
                    })
                    .Subscribe();
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

            _networkManager.Discover(data, seq);

            //_nm.Discover(data, seq)
            //    .Skip(1)
            //    .Subscribe(result =>
            //        {
            //            CreateBulbFromUdpResult(result);
            //        },
            //        () =>
            //        {
            //            Bulbs.ForEach(b =>
            //            {
            //                seq = SequenceGenerator.GetNext();

            //                data = new byte[PacketSize.LABEL];
            //                GetLabelPacket(b.SiteAddress, seq, data);
            //                _nm.GetLabel(data, seq, b.IP)
            //                    .Subscribe(result =>
            //                    {
            //                        var labelBytes = new byte[32];
            //                        Array.Copy(result.Buffer, 36, labelBytes, 0, 32);
            //                        b.Label = Encoding.UTF8.GetString(labelBytes);
            //                    });
            //            });
            //        });

            /*
            var labels = new IntPtr[NUM_BULBS];
            var groups = new IntPtr[NUM_BULBS];

            for (var i = 0; i < labels.Length; ++i)
            {
                labels[i] = Marshal.AllocHGlobal(256);
                groups[i] = Marshal.AllocHGlobal(256);
            }

            GetLabels(labels);
            GetGroups(groups);

            var label_names = new string[labels.Length];
            var group_names = new string[groups.Length];
            for (var i = 0; i < labels.Length; ++i)
            {
                label_names[i] = Marshal.PtrToStringUni(labels[i]);
                group_names[i] = Marshal.PtrToStringUni(groups[i]);
                Marshal.FreeHGlobal(labels[i]);
                Marshal.FreeHGlobal(groups[i]);
                labels[i] = IntPtr.Zero;
                groups[i] = IntPtr.Zero;

                Bulbs.Add(new LifxBulb(label_names[i])
                {
                    Group = group_names[i]
                });
            }
            */
        }

		public async Task GetLightState(string label = null)
		{
			if (label != null)
			{
				Bulbs.Where(b => b.Label == label)
					.ToList()
					.ForEach(b => 
					{
                        var state = new uint[6];
                        GetLightState(b.Label, state);

                        b.Hue = (float)state[0] * 360 / ushort.MaxValue;
                        b.Saturation = (float)state[1] / ushort.MaxValue;
                        b.Brightness = (float)state[2] / ushort.MaxValue;
                        b.Kelvin = (ushort)state[3];
                        b.Dim = (ushort)state[4];
                        b.Power = (ushort)state[5];
					});
			}
			else
			{
				Bulbs.ForEach(b =>
						{
                            try {
                                var state = new uint[6];
                                GetLightState(b.Label, state);

                                b.Hue = (float)state[0] * 360 / ushort.MaxValue;
                                b.Saturation = (float)state[1] / ushort.MaxValue;
                                b.Brightness = (float)state[2] / ushort.MaxValue;
                                b.Kelvin = (ushort)state[3];
                                b.Dim = (ushort)state[4];
                                b.Power = (ushort)state[5];
                            } 
                            catch
                            {
                                Debugger.Break();
                            }
						});
			}
		}

		public async Task SetLightState(float hue, float saturation, float brightness, ushort kelvin, ushort dim, string target = null)
		{
			if (kelvin > 9000 || kelvin < 2500) {
				throw new ArgumentOutOfRangeException(nameof(kelvin),
													kelvin,
													"Kelvin should be betweeen 2500 & 9000");
			}

			var fhue = hue % 360;
			var fsaturation = saturation % 100;
			var fbrightness = brightness % 100;

			var state = new ushort[5];

			state[0] = Convert.ToUInt16(fhue * ushort.MaxValue / 360); // hue * max / 360
			state[1] = Convert.ToUInt16(fsaturation * ushort.MaxValue); // %
			state[2] = Convert.ToUInt16(fbrightness * ushort.MaxValue); // %
			state[3] = kelvin; // between 2500 & 9000
			state[4] = dim;

			if (target != null)
			{
				Bulbs.Where(b => b.Label == target)
                     .ToList()
                     .ForEach(b => SetLightColor(b.Label, state));
			}
			else
			{
				Bulbs.ForEach(b => SetLightColor(b.Label, state));
			}
		}

		public async Task<ushort> SetPower(ushort onoff, string target = null)
		{
			if (target != null)
			{
                var bulb = Bulbs.Where(b => b.Label == target)
                      .ToList()
                      .FirstOrDefault();
                      
                SetPower(bulb.Label, onoff);
                return await GetPower(bulb.Label);
			}
			else
			{
				Bulbs.ForEach(/*async*/ b => {
                    SetPower(b.Label, onoff);
                    //await GetPower(b.Label);
                });
			}

            return 0;
		}

        public Task<ushort> GetPower(string target)
        {
            if (target != null)
            {
                var power = new ushort[1];
                var bulb = Bulbs.Where(b => b.Label == target)
                    .ToList()
                    .FirstOrDefault();
                GetPower(bulb.Label, power);
                bulb.Power = power[0];

                return Task.FromResult((ushort)power[0]);
            }

            return Task.FromResult((ushort)0);
        }
	}
}