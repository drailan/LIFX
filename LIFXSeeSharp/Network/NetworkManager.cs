using LIFXSeeSharp.Extensions;
using LIFXSeeSharp.Helpers;
using LIFXSeeSharp.Logging;
using LIFXSeeSharp.Packet;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;

namespace LIFXSeeSharp.Network
{
	class NetworkManager : IDisposable
	{
		private readonly string TAG = "NetworkManager";

		private int _port;
		private UdpClient _udp;
		private IPEndPoint _bcastEP;

		public NetworkManager(int port = 56700)
		{
			_port = port;

			_bcastEP = new IPEndPoint(IPAddress.Broadcast, _port);
			InitializeSender();
		}

		private void InitializeSender()
		{
			_udp = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
			_udp.EnableBroadcast = true;
			_udp.Client.MulticastLoopback = false;
		}

		public IObservable<IPacket> UdpListener()
		{
			return Observable.Create<IPacket>(async (observer, token) =>
			{
				while (true)
				{
					if (token.IsCancellationRequested) { return; }
					try
					{
						var udpResult = await _udp.ReceiveAsync();

						var packetTypeSubArray = new byte[2];
						Array.Copy(udpResult.Buffer, 32, packetTypeSubArray, 0, 2);
						var packetType = BitConverter.ToUInt16(packetTypeSubArray, 0);

						var creator = packetType.ToPacket();
						observer.OnNext(creator(udpResult.Buffer, udpResult.RemoteEndPoint.Address));
					}
					catch (ObjectDisposedException e)
					{
						Log.Error(TAG, "Udp is disposed, trying to exit?");
						continue;
					}
				}
			});
		}

		public void Discover(byte[] packet, byte seq, int numBulbs = 4)
		{
			_udp.Send(packet, PacketSize.DISCOVERY, _bcastEP);
			Log.Debug(TAG, "Sent discovery packet with sequence {0}", seq);
		}

		public void GetLabel(byte[] packet, byte seq, IPAddress ip)
		{
			IPEndPoint ep = new IPEndPoint(ip, _port);
			_udp.Send(packet, PacketSize.LABEL, ep);
			Log.Debug(TAG, "Sent label packet with sequence {0}", seq);
		}

		public void Dispose()
		{
			_udp.Dispose();
		}
	}
}
