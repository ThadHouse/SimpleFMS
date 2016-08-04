using System;
using System.Collections.Generic;
using System.Threading;
using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Networking;
using SimpleFMS.Networking.Server.NetworkTableUpdaters;

namespace SimpleFMS.Networking.Server
{
    public class NetworkServerManager : INetworkServerManager
    {
        private readonly List<NetworkTableUpdaterBase> m_networkTableUpdaters = new List<NetworkTableUpdaterBase>();

        private readonly object m_lockObject = new object();

        private const int TableUpdatePeriod = 500;

        private readonly IDriverStationManager m_driverStationManager;

        private readonly ITable m_networkTableRoot;

        private readonly Timer m_updateTimer;

        public NetworkServerManager(IDriverStationManager driverStationManager)
        {
            m_driverStationManager = driverStationManager;

            NetworkTable.SetServerMode();
            NetworkTable.SetPort(NetworkingConstants.NetworkTablesPort);
            NetworkTable.SetNetworkIdentity(NetworkingConstants.ServerRemoteName);
            NetworkTable.SetUpdateRate(1.0);
            NetworkTable.Initialize();

            m_networkTableRoot = NetworkTable.GetTable(NetworkingConstants.RootTableName);

            m_networkTableUpdaters.Add(new DriverStationNetworkTableUpdater(m_networkTableRoot, m_driverStationManager));


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
