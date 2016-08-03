using System;
using System.Net;
using System.Net.Sockets;
using SimpleFMS.Base.DriverStation.Interfaces;
using SimpleFMS.DriverStation.UdpData;

namespace SimpleFMS.DriverStation.UdpControllers
{
    public class DriverStationControlSender : IRestartable
    {
        private UdpClient m_client;

        public DriverStationControlSender()
        {
            m_client = new UdpClient();
        }

        public void SendPacket(IPEndPoint ipEp, DriverStationControlData data)
        {
            try
            {
                byte[] packet = data.PackData();
                m_client.Send(packet, packet.Length, ipEp);
            }
            catch (SocketException)
            {
                // TODO: Log
            }
            catch (ObjectDisposedException)
            {
                // Ignore Disposed Exception
            }
        }

        public void Restart()
        {
            Dispose();
            m_client = new UdpClient();
        }

        public void Dispose()
        {
            m_client?.Dispose();
            m_client = null;
        }
    }
}
