using System;
using System.Collections.Generic;
using System.Threading;
using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.MatchTiming;
using SimpleFMS.Base.Networking;
using SimpleFMS.Networking.Server.NetworkTableUpdaters;

namespace SimpleFMS.Networking.Server
{
    public class NetworkServerManager : INetworkServerManager
    {
        private readonly List<NetworkTableUpdaterBase> m_networkTableUpdaters = new List<NetworkTableUpdaterBase>();

        private readonly object m_lockObject = new object();

        private const int TableUpdatePeriod = 500;

        private readonly ITable m_networkTableRoot;

        private readonly Timer m_updateTimer;

        public NetworkServerManager(IDriverStationManager driverStationManager, IMatchTimingManager matchTimingManager)
        {

            NetworkTable.SetServerMode();
            NetworkTable.SetPort(NetworkingConstants.NetworkTablesPort);
            NetworkTable.SetNetworkIdentity(NetworkingConstants.ServerRemoteName);
            NetworkTable.SetUpdateRate(1.0);
            NetworkTable.Initialize();

            m_networkTableRoot = NetworkTable.GetTable(NetworkingConstants.RootTableName);

            m_networkTableUpdaters.Add(new DriverStationUpdater(m_networkTableRoot, driverStationManager));
            m_networkTableUpdaters.Add(new MatchTimingUpdater(m_networkTableRoot, matchTimingManager));

            m_updateTimer = new Timer(OnTimerUpdate);
            m_updateTimer.Change(TableUpdatePeriod, TableUpdatePeriod);
        }

        public void Dispose()
        {
            m_updateTimer.Dispose();

            lock (m_lockObject)
            {
                foreach (var updater in m_networkTableUpdaters)
                {
                    updater.Dispose();
                }
            }

            NetworkTable.Shutdown();
        }

        private void OnTimerUpdate(object state)
        {
            lock (m_lockObject)
            {
                foreach (var updater in m_networkTableUpdaters)
                {
                    updater.UpdateTable();
                }
            }
            NetworkTable.Flush();
        }
    }
}
