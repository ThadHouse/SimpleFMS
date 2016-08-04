using System.Net;
using System.Threading;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.DriverStation.Interfaces;
using SimpleFMS.DriverStation.UdpControllers;
using SimpleFMS.DriverStation.UdpData;

namespace SimpleFMS.DriverStation
{
    public class DriverStation
    {
        public AllianceStation Station 
        {
            get { return ControlData.Station; }
            set { ControlData.Station = value; }
        }

        public bool IsEStopped
        {
            get { return ControlData.IsEStopped; }
            set { ControlData.IsEStopped = value; }
        }

        public bool IsBypassed { get; set; }

        public int TeamNumber { get; set; }

        public DriverStationStatusData StatusResult { get; internal set; } = null;

        private IPEndPoint m_ipEp = null;
        private readonly int m_port = 0;
        private readonly DriverStationControlSender m_client = null;

        public DriverStationControlData ControlData { get; } = new DriverStationControlData();

        public DriverStation(int port, DriverStationControlSender client)
        {
            m_ipEp = null;
            m_port = port;
            m_client = client;
        }

        internal IDriverStationReport GenerateDriverStationReport()
        {
            var dsReport = new DriverStationReport()
            {
                TeamNumber = TeamNumber,
                Station = Station,
                IsBeingSentAutonomous = GlobalDriverStationControlData.IsAutonomous,
                IsBeingSentEStopped = IsEStopped,
                IsBeingSentEnabled = ControlData.IsEnabled,
                DriverStationConnected = m_ipEp != null,
                // TODO: Add These
                IsReceivingAutonomous = false,
                IsReceivingEStopped = false,
                IsReceivingEnabled = false,
                RoboRioConnected = false,
                RobotBattery = 0.0
            };
            return dsReport;
        }

        internal void ConnectDriverStation(IPAddress address)
        {
            IPEndPoint ipEp = new IPEndPoint(address, m_port);
            // Only update IpAdress if old address is null
            Interlocked.CompareExchange(ref m_ipEp, ipEp, null);
        }

        internal void DisconnectDriverStation()
        {
            Interlocked.Exchange(ref m_ipEp, null);
        }

        public void SendPacket()
        {
            IPEndPoint ipEp = null;
            Interlocked.Exchange(ref ipEp, m_ipEp);
            if (ipEp == null) return;
            ControlData.IsEnabled = !IsEStopped && !IsBypassed && GlobalDriverStationControlData.IsEnabled;
            m_client?.SendPacket(ipEp, ControlData);
        }
    }
}
