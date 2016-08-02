using System;
using System.Net;
using System.Net.Sockets;
using SimpleFMS.DriverStation.Base.Interfaces;
using SimpleFMS.DriverStation.UdpData;

namespace SimpleFMS.DriverStation.UdpControllers
{
    public class DriverStationStatusReceiver : IRestartable
    {
        private UdpClient m_client = null;
        private IPEndPoint m_endPoint;
        private readonly object m_lockObject = new object();

        public event Action<DriverStationStatusData> OnDriverStationReceive;

        public DriverStationStatusReceiver(int port)
        {
            m_endPoint = new IPEndPoint(IPAddress.Any, port);
        }

        public void Restart()
        {
            m_client?.Close();
            m_client?.Dispose();

            m_client = new UdpClient(m_endPoint);

            m_client.BeginReceive(OnReceive, this);
        }

        private static void OnReceive(IAsyncResult result)
        {
            DriverStationStatusReceiver rec = result.AsyncState as DriverStationStatusReceiver;
            if (rec == null) return;
            lock (rec.m_lockObject)
            {
                if (rec.m_client == null) return;
                try
                {
                    byte[] data = rec.m_client.EndReceive(result, ref rec.m_endPoint);
                    DriverStationStatusData parsedData = new DriverStationStatusData();
                    parsedData.ParseData(data);
                    rec.OnDriverStationReceive?.Invoke(parsedData);
                    rec.m_client.BeginReceive(OnReceive, rec);
                }
                catch (ObjectDisposedException)
                {
                    // Ignore an Object Disposed Exception
                }
            }
        }

        public void Dispose()
        {
            OnDriverStationReceive = null;
            m_client?.Close();
            m_client?.Dispose();
            m_client = null;
        }
    }
}
