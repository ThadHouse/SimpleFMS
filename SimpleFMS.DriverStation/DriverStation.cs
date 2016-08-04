using System;
using System.Net;
using System.Threading;
using SimpleFMS.Base.DriverStation;
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

        public bool IsReceivingAutonomous { get; private set; } = true;
        public bool IsReceivingEnabled { get; private set; }
        public bool IsReceivingEStopped { get; private set; }
        public bool IsRoboRioConnected { get; private set; }
        public bool DriverStationConnected { get; set; }

        public double RobotBattery { get; private set; }

        public const int WatchDogTimer = 1500; // Milliseconds

        //public DriverStationStatusData StatusResult { get; internal set; } = null;

        private IPEndPoint m_ipEp = null;
        private readonly int m_port = 0;
        private readonly DriverStationControlSender m_client = null;

        private Timer m_watchDogTimer;

        public DriverStationControlData ControlData { get; } = new DriverStationControlData();

        public DriverStation(int port, DriverStationControlSender client)
        {
            m_ipEp = null;
            m_port = port;
            m_client = client;
            m_watchDogTimer = new Timer(OnWatchDogExpiration);
            m_watchDogTimer.Change(WatchDogTimer, 0);
        }

        private void OnWatchDogExpiration(object state)
        {
            DriverStationConnected = false;
        }

        private void FeedWatchDog()
        {
            m_watchDogTimer.Change(WatchDogTimer, 0);
        }

        internal void UpdateDriverStationResponse(DriverStationStatusData data)
        {
            if (data == null) return;
            FeedWatchDog();

            DriverStationConnected = true;

            IsRoboRioConnected = data.HasRobotComms;
            IsReceivingEStopped = data.IsEStopped;
            IsReceivingAutonomous = data.IsAutonomous;
            IsReceivingEnabled = data.IsEnabled;
            RobotBattery = data.RobotBattery;
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
                DriverStationConnected = DriverStationConnected,
                // TODO: Add These
                IsReceivingAutonomous = IsReceivingAutonomous,
                IsReceivingEStopped = IsReceivingEStopped,
                IsReceivingEnabled = IsReceivingEnabled,
                RoboRioConnected = IsRoboRioConnected,
                RobotBattery = RobotBattery
            };
            return dsReport;
        }

        internal IPAddress GetRemoteIpAddress()
        {
            IPEndPoint ipEp = m_ipEp;
            return ipEp?.Address;
        }

        internal void ConnectDriverStation(IPAddress address)
        {
            IPEndPoint ipEp = new IPEndPoint(address, m_port);
            // Only update IpAdress if old address is null
            Interlocked.CompareExchange(ref m_ipEp, ipEp, null);
            DriverStationConnected = true;
            FeedWatchDog();
        }

        internal void DisconnectDriverStation()
        {
            Interlocked.Exchange(ref m_ipEp, null);
            IsRoboRioConnected = false;
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
