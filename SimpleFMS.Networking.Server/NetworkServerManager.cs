using System.Collections.Generic;
using System.Threading;
using NetworkTables;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.MatchTiming;
using SimpleFMS.Base.Networking;
using SimpleFMS.Networking.Server.NetworkTableUpdaters;
using static SimpleFMS.Base.Networking.NetworkingConstants;

namespace SimpleFMS.Networking.Server
{
    public class NetworkServerManager : INetworkServerManager
    {
        private readonly List<NetworkTableUpdaterBase> m_networkTableUpdaters = new List<NetworkTableUpdaterBase>();

        private readonly object m_lockObject = new object();

        private const int TableUpdatePeriod = 500;

        private readonly StandaloneNetworkTable m_networkTableRoot;
        private readonly StandaloneNtCore m_standaloneNtCore;
        private readonly StandaloneRemoteProcedureCall m_standaloneRpc;

        private readonly Timer m_updateTimer;

        public NetworkServerManager(IDriverStationManager driverStationManager, IMatchTimingManager matchTimingManager)
        {
            m_standaloneNtCore = new StandaloneNtCore();
            m_standaloneNtCore.UpdateRate = 1.0;
            m_standaloneNtCore.RemoteName = ServerRemoteName;
            m_standaloneNtCore.StartServer(PersistentFilename, "", StandaloneNtCore.DefaultPort);
            m_standaloneRpc = new StandaloneRemoteProcedureCall(m_standaloneNtCore);
            m_networkTableRoot = new StandaloneNetworkTable(m_standaloneNtCore, RootTableName);

            m_networkTableUpdaters.Add(new DriverStationUpdater(m_networkTableRoot,m_standaloneRpc, driverStationManager));
            m_networkTableUpdaters.Add(new MatchTimingUpdater(m_networkTableRoot, m_standaloneRpc, matchTimingManager));

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
