using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using SimpleFMS.DriverStation.Base.Enums;
using SimpleFMS.DriverStation.Base.Interfaces;
using SimpleFMS.DriverStation.Base.Tuples;
using SimpleFMS.DriverStation.TcpControllers;
using SimpleFMS.DriverStation.UdpControllers;
using SimpleFMS.DriverStation.UdpData;

namespace SimpleFMS.DriverStation
{
    public class DriverStationManager : IDriverStationManager, IDisposable
    {
        // Dictionaries to hold our team values
        private readonly Dictionary<int, ImmutableStructTuple<AllianceStationSide, AllianceStationNumber>>
            m_allianceStationsByTeam =
                new Dictionary<int, ImmutableStructTuple<AllianceStationSide, AllianceStationNumber>>(
                    GlobalDriverStationSettings.MaxNumDriverStations);

        private readonly Dictionary<ImmutableStructTuple<AllianceStationSide, AllianceStationNumber>, int>
            m_teamsByAllianceStations =
                new Dictionary<ImmutableStructTuple<AllianceStationSide, AllianceStationNumber>, int>(
                    GlobalDriverStationSettings.MaxNumDriverStations);

        private readonly Dictionary<int, DriverStation> m_driverStationByTeam =
            new Dictionary<int, DriverStation>(GlobalDriverStationSettings.MaxNumDriverStations);

        public IReadOnlyList<IDriverStationConfiguration> ConnectedDriverStations => m_driverStationByTeam.Values.ToList();

        public IReadOnlyDictionary<ImmutableStructTuple<AllianceStationSide, AllianceStationNumber>, int>
            RequestedDriverStations => m_teamsByAllianceStations;

        private readonly object m_lockObject = new object();

        private readonly Timer m_updateStationsTimer;
        private readonly Timer m_updateNetworkTimer;

        private readonly DriverStationConnectionListener m_connectionListener;

        private readonly DriverStationControlSender m_dsControlSender;
        private readonly DriverStationStatusReceiver m_dsStatusReceiver;

        private IDriverStationNetworkSender m_networkSender = null;
        private IDriverStationConnectionResponder m_connectionResponder = null;

        public DriverStationManager(IDriverStationNetworkSender networkSender, IDriverStationConnectionResponder connectionResponder)
        {
            m_networkSender = networkSender;
            m_connectionResponder = connectionResponder;

            m_allianceStationsByTeam.Clear();
            m_teamsByAllianceStations.Clear();
            m_driverStationByTeam.Clear();
            m_dsStatusReceiver = new DriverStationStatusReceiver(GlobalDriverStationSettings.UdpReceivePort);
            m_dsStatusReceiver.Restart();
            m_dsStatusReceiver.OnDriverStationReceive += OnDriverStationStatusReceive;

            m_dsControlSender = new DriverStationControlSender();

            m_connectionListener = new DriverStationConnectionListener(GlobalDriverStationSettings.TcpListenPort);
            m_connectionListener.OnNewDriverStationConnected += OnDriverStationConnected;
            m_connectionListener.Restart();

            m_updateNetworkTimer = new Timer(state =>
            {
                var stationLists =
                    new List<ImmutableStructTuple<IDriverStationIncomingData, IDriverStationOutgoingData>>();
                lock (m_lockObject)
                {
                    foreach (var driverStation in m_driverStationByTeam)
                    {
                        var value = driverStation.Value;
                        var ds = new ImmutableStructTuple<IDriverStationIncomingData, IDriverStationOutgoingData>(
                            value.StatusResult, value.ControlData);
                        stationLists.Add(ds);
                    }
                }
                m_networkSender?.UpdateDriverStationNeworkData(stationLists);
                m_networkSender?.UpdateRequestedDriverStations(RequestedDriverStations);
            });

            m_updateNetworkTimer.Change(500, 500);

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
            m_driverStationByTeam.Clear();
            m_allianceStationsByTeam.Clear();
            m_teamsByAllianceStations.Clear();
            m_connectionListener?.Dispose();
            m_updateStationsTimer?.Dispose();
            m_dsStatusReceiver?.Dispose();
            m_dsControlSender?.Dispose();
        }

        private void OnDriverStationConnected(int teamNumber, IPAddress ipAddress, out AllianceStationSide allianceSide, out AllianceStationNumber stationNumber, out bool isRequested)
        {
            lock (m_lockObject)
            {
                DriverStation existingDs = null;
                ImmutableStructTuple<AllianceStationSide, AllianceStationNumber> existingLocation;
                if (m_driverStationByTeam.TryGetValue(teamNumber, out existingDs))
                {
                    // Already contains DS
                    allianceSide = existingDs.AllianceSide;
                    stationNumber = existingDs.StationNumber;
                    isRequested = true;
                }
                else if (m_allianceStationsByTeam.TryGetValue(teamNumber, out existingLocation))
                {
                    // A requested team number
                    DriverStation ds = new DriverStation(ipAddress, GlobalDriverStationSettings.UdpSendPort, m_dsControlSender);
                    allianceSide = existingLocation.First;
                    stationNumber = existingLocation.Second;
                    ds.StationNumber = stationNumber;
                    ds.AllianceSide = allianceSide;
                    isRequested = true;
                    m_driverStationByTeam.Add(teamNumber, ds);
                    m_connectionResponder?.OnDriverStationConnectionChanged(ds, true);
                }
                else
                {
                    allianceSide = 0;
                    stationNumber = 0;
                    isRequested = false;
                }
            }
        }

        private void OnDriverStationStatusReceive(DriverStationStatusData statusData)
        {
            DriverStation ds = null;
            lock (m_lockObject)
            {
                if (m_driverStationByTeam.TryGetValue(statusData.TeamNumber, out ds))
                {
                    m_connectionResponder?.OnRobotConnectionChanged(ds, statusData.HasRobotComms);
                }
            }
        }

        private void UpdateDriverStations()
        {
            var values = m_driverStationByTeam.Values;
            var now = DateTime.UtcNow;
            GlobalDriverStationControlData.FmsTime = now;
            foreach (var value in values)
            {
                value.SendPacket();
            }
        }

        public bool InitializeMatch(IReadOnlyList<IDriverStationConfiguration> driverStationConfigurations, int matchNumber, MatchType matchType)
        {
            lock (m_lockObject)
            {
                StopMatchPeriod();
                m_driverStationByTeam.Clear();
                m_allianceStationsByTeam.Clear();
                m_teamsByAllianceStations.Clear();
                m_connectionListener.Restart();

                m_connectionResponder?.AllConnectionReset();

                // Only allow a max of 6 driver stations
                if (driverStationConfigurations.Count > GlobalDriverStationSettings.MaxNumDriverStations) return false;
                GlobalDriverStationControlData.MatchNumber = matchNumber;
                GlobalDriverStationControlData.MatchType = matchType;

                foreach (var driverStationConfiguration in driverStationConfigurations)
                {
                    if (driverStationConfiguration.IsBypassed) continue;
                    var configData =
                        new ImmutableStructTuple<AllianceStationSide, AllianceStationNumber>(
                            driverStationConfiguration.AllianceSide, driverStationConfiguration.StationNumber);

                    m_teamsByAllianceStations.Add(configData, driverStationConfiguration.TeamNumber);
                    m_allianceStationsByTeam.Add(driverStationConfiguration.TeamNumber, configData);
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

        public void SetBypass(AllianceStationSide alliance, AllianceStationNumber station, bool bypassed)
        {
            var configData = new ImmutableStructTuple<AllianceStationSide, AllianceStationNumber>(alliance, station);
            int teamNumber = 0;
            lock (m_lockObject)
            {
                if (m_teamsByAllianceStations.TryGetValue(configData, out teamNumber))
                {
                    DriverStation ds = null;
                    if (m_driverStationByTeam.TryGetValue(teamNumber, out ds))
                    {
                        ds.IsBypassed = bypassed;
                    }
                }
            }
        }

        public void SetEStop(AllianceStationSide alliance, AllianceStationNumber station, bool eStopped)
        {
            var configData = new ImmutableStructTuple<AllianceStationSide, AllianceStationNumber>(alliance, station);
            int teamNumber = 0;
            lock (m_lockObject)
            {
                if (m_teamsByAllianceStations.TryGetValue(configData, out teamNumber))
                {
                    DriverStation ds = null;
                    if (m_driverStationByTeam.TryGetValue(teamNumber, out ds))
                    {
                        ds.IsEStopped = eStopped;
                    }
                }
            }
        }
    }
}
