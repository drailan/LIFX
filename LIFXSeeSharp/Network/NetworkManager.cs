using LifxSeeSharp.Extensions;
using LifxSeeSharp.Helpers;
using LifxSeeSharp.Packet;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Linq;

namespace LifxSeeSharp.Network
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
						var packet = creator(udpResult.Buffer, udpResult.RemoteEndPoint.Address);
						packet.ProcessPayload();

						observer.OnNext(packet);
					}
					catch (ObjectDisposedException e)
					{
						Log.Error(TAG, "Udp is disposed, trying to exit? Exception is: " + e.Message);
						continue;
					}
				}
			});
		}

		public void Discover(byte[] packet, byte seq)
		{
			_udp.Send(packet, PacketSize.DISCOVERY, _bcastEP);
			Log.Debug(TAG, "Sent discovery packet with sequence {0}", seq);
		}

		public void SendTargetedPacket(byte[] packet, byte seq, IPAddress ip)
		{
			IPEndPoint ep = new IPEndPoint(ip, _port);
			_udp.Send(packet, packet.Length, ep);
			Log.Debug(TAG, "Sent packet with sequence {0} to {1}", seq, ip);
		}

		public void Dispose()
		{
			_udp.Dispose();
		}
	}
}
