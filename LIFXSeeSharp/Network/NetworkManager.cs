using LIFXSeeSharp.Extensions;
using LIFXSeeSharp.Helpers;
using LIFXSeeSharp.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LIFXSeeSharp.Network
{
    class NetworkManager
    {
        private int _port;
        private UdpClient _udpSender;
        private IPEndPoint _bcastEP;

        public NetworkManager(int port = 56700) {
            _port = port;

            _bcastEP = new IPEndPoint(IPAddress.Broadcast, _port);
            InitializeSender();
        }

        private void InitializeSender()
        {
            _udpSender = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
            _udpSender.EnableBroadcast = true;
            _udpSender.Client.MulticastLoopback = false;
        }

        public IObservable<IPacket> UdpListener()
        {
            return Observable.Create<IPacket>(async (observer, token) =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested) { return; }
                    var udpResult = await _udpSender.ReceiveAsync();

                    var packetTypeSubArray = new byte[2];
                    Array.Copy(udpResult.Buffer, 32, packetTypeSubArray, 0, 2);
                    var packetType = BitConverter.ToUInt16(packetTypeSubArray, 0);

                    var creator = packetType.ToPacket();
                    observer.OnNext(creator(udpResult.Buffer, udpResult.RemoteEndPoint.Address));
                }
            });
        }

        public void Discover(byte[]packet, byte seq, int numBulbs = 4)
        {
            _udpSender.Send(packet, PacketSize.DISCOVERY, _bcastEP);
        }

        public void GetLabel(byte[] packet, byte seq, IPAddress ip)
        {
            IPEndPoint ep = new IPEndPoint(ip, _port);
            _udpSender.Send(packet, PacketSize.LABEL, ep);
        }
    }
}
