using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.DriverStation.TcpControllers;
using SimpleFMS.DriverStation.UdpControllers;
using SimpleFMS.DriverStation.UdpData;

namespace SimpleFMS.DriverStation
{
    public class DriverStationManager : IDriverStationManager
    {
        private readonly Dictionary<AllianceStation, DriverStation> m_driverStationsByAllianceStation =
            new Dictionary<AllianceStation, DriverStation>(AllianceStationConstants.MaxNumDriverStations);


        private readonly Dictionary<int, DriverStation> m_driverStationsByTeam =
            new Dictionary<int, DriverStation>(AllianceStationConstants.MaxNumDriverStations);


        public event Action<IReadOnlyDictionary<AllianceStation, IDriverStationReport>> OnDriverStationStatusChanged;

        public IReadOnlyDictionary<AllianceStation, IDriverStationReport> DriverStations
        {
            get
            {
                lock (m_lockObject)
                {
                    Dictionary<AllianceStation, IDriverStationReport> reports =
                        new Dictionary<AllianceStation, IDriverStationReport>(m_driverStationsByTeam.Count);
                    foreach (var ds in m_driverStationsByTeam.Values)
                    {
                        reports.Add(ds.Station, ds.GenerateDriverStationReport());
                    }
                    return reports;
                }
            }
        }

        private readonly object m_lockObject = new object();

        private readonly Timer m_updateStationsTimer;

        private readonly DriverStationConnectionListener m_connectionListener;

        private readonly DriverStationControlSender m_dsControlSender;
        private readonly DriverStationStatusReceiver m_dsStatusReceiver;

        public DriverStationManager()
        {
            m_dsStatusReceiver = new DriverStationStatusReceiver(GlobalDriverStationSettings.UdpReceivePort);
            m_dsStatusReceiver.Restart();
            m_dsStatusReceiver.OnDriverStationReceive += OnDriverStationStatusReceive;

            m_dsControlSender = new DriverStationControlSender();

            m_connectionListener = new DriverStationConnectionListener(GlobalDriverStationSettings.TcpListenPort);
            m_connectionListener.OnNewDriverStationConnected += OnDriverStationConnected;
            m_connectionListener.Restart();

            m_updateStationsTimer = new Timer(state =>
            {
                lock (m_lockObject)
                {
                    UpdateDriverStations();
                }
            });

            m_updateStationsTimer.Change(250, 250);
        }

        public void Dispose()
        {
            m_driverStationsByAllianceStation?.Clear();
            m_driverStationsByTeam?.Clear();
            m_connectionListener?.Dispose();
            m_updateStationsTimer?.Dispose();
            m_dsStatusReceiver?.Dispose();
            m_dsControlSender?.Dispose();
        }

        private void OnDriverStationConnected(int teamNumber, IPAddress ipAddress, out AllianceStation station, out bool isRequested)
        {
            bool updateConnections = false;
            lock (m_lockObject)
            {
                DriverStation existingDs = null;
                if (m_driverStationsByTeam.TryGetValue(teamNumber, out existingDs))
                {
                    IPAddress oldIpAddress = existingDs.GetRemoteIpAddress();
                    // Team is a currently requested team.
                    existingDs.ConnectDriverStation(ipAddress);
                    station = existingDs.Station;
                    if (!ipAddress.Equals(oldIpAddress))
                    {
                        updateConnections = true;
                    }
                    isRequested = true;
                }
                else
                {
                    // Driver Station is not requested.
                    station = new AllianceStation();
                    isRequested = false;
                }
            }
            if (updateConnections)
            {
                OnDriverStationStatusChanged?.Invoke(DriverStations);
            }
        }

        private void OnDriverStationStatusReceive(DriverStationStatusData statusData)
        {
            if (statusData == null) return;
            lock (m_lockObject)
            {
                DriverStation ds = null;
                if (m_driverStationsByTeam.TryGetValue(statusData.TeamNumber, out ds))
                {
                    ds.UpdateDriverStationResponse(statusData);
                }
            }
        }

        private void UpdateDriverStations()
        {
            var values = m_driverStationsByTeam.Values;
            var now = DateTime.UtcNow;
            GlobalDriverStationControlData.FmsTime = now;
            foreach (var value in values)
            {
                value.SendPacket();
            }
            OnDriverStationStatusChanged?.Invoke(DriverStations);
        }

        public bool InitializeMatch(IReadOnlyList<IDriverStationConfiguration> driverStationConfigurations, int matchNumber, MatchType matchType)
        {
            lock (m_lockObject)
            {
                StopMatchPeriod();
                m_driverStationsByAllianceStation?.Clear();
                m_driverStationsByTeam?.Clear();
                m_connectionListener.Restart();

                // Only allow a max of 6 driver stations
                if (driverStationConfigurations.Count > AllianceStationConstants.MaxNumDriverStations) return false;
                GlobalDriverStationControlData.MatchNumber = matchNumber;
                GlobalDriverStationControlData.MatchType = matchType;

                if (m_driverStationsByTeam == null || m_driverStationsByAllianceStation == null) return false;

                foreach (var driverStationConfiguration in driverStationConfigurations)
                {
                    DriverStation ds = new DriverStation(GlobalDriverStationSettings.UdpSendPort, m_dsControlSender)
                    {
                        IsBypassed = driverStationConfiguration.IsBypassed,
                        Station = driverStationConfiguration.Station,
                        TeamNumber = driverStationConfiguration.TeamNumber
                    };
                    m_driverStationsByAllianceStation.Add(driverStationConfiguration.Station, ds);
                    m_driverStationsByTeam.Add(driverStationConfiguration.TeamNumber, ds);
                }
                return true;
            }
        }

        public void StartMatchPertiod(bool auto)
        {
            lock (m_lockObject)
            {
                GlobalDriverStationControlData.IsAutonomous = auto;
                GlobalDriverStationControlData.IsEnabled = true;
                UpdateDriverStations();
            }
        }

        public void StopMatchPeriod()
        {
            lock (m_lockObject)
            {
                GlobalDriverStationControlData.IsEnabled = false;
                UpdateDriverStations();
            }
        }

        public void SetRemainingMatchTime(int remainingMatchTime)
        {
            if (remainingMatchTime < 0) remainingMatchTime = 0;
            GlobalDriverStationControlData.MatchTimeRemaining = remainingMatchTime;
        }

        public void SetBypass(AllianceStation station, bool bypassed)
        {
            lock (m_lockObject)
            {
                DriverStation ds;
                if (m_driverStationsByAllianceStation.TryGetValue(station, out ds))
                {
                    ds.IsBypassed = bypassed;
                }
            }
        }

        public void SetEStop(AllianceStation station, bool eStopped)
        {
            lock (m_lockObject)
            {
                DriverStation ds;
                if (m_driverStationsByAllianceStation.TryGetValue(station, out ds))
                {
                    ds.IsEStopped = eStopped;
                }
            }
        }
    }
}
