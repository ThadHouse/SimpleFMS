using System.Net;
using SimpleFMS.Base.DriverStation.Enums;
using SimpleFMS.Base.DriverStation.Interfaces;
using SimpleFMS.DriverStation.UdpControllers;
using SimpleFMS.DriverStation.UdpData;

namespace SimpleFMS.DriverStation
{
    public class DriverStation : IDriverStationConfiguration
    {
        public AllianceStationSide AllianceSide
        {
            get { return ControlData.AllianceSide; }
            set { ControlData.AllianceSide = value; }
        }

        public AllianceStationNumber StationNumber
        {
            get { return ControlData.StationNumber; }
            set { ControlData.StationNumber = value; }
        }

        public bool IsEStopped
        {
            get { return ControlData.IsEStopped; }
            set { ControlData.IsEStopped = value; }
        }

        public bool IsBypassed { get; set; }

        public int TeamNumber { get; set; }

        public DriverStationStatusData StatusResult { get; internal set; } = null;

        private readonly IPEndPoint m_ipEp = null;
        private readonly DriverStationControlSender m_client = null;

        public DriverStationControlData ControlData { get; } = new DriverStationControlData();

        public DriverStation(IPAddress address, int port, DriverStationControlSender client)
        {
            m_ipEp = new IPEndPoint(address, port);
            m_client = client;
            ControlData.IpAddress = address;
        }

        public void SendPacket()
        {
            if (m_ipEp == null) return;
            ControlData.IsEnabled = !IsEStopped && !IsBypassed && GlobalDriverStationControlData.IsEnabled;
            m_client?.SendPacket(m_ipEp, ControlData);
        }
    }
}
