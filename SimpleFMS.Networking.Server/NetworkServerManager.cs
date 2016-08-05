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

        private const int TableUpdatePeriod = 100;

        private readonly StandaloneNetworkTable m_networkTableRoot;
        private readonly StandaloneNtCore m_ntCore;
        private readonly StandaloneRemoteProcedureCall m_rpc;

        private readonly Timer m_updateTimer;

        public NetworkServerManager(IDriverStationManager driverStationManager, IMatchTimingManager matchTimingManager)
        {
            m_ntCore = new StandaloneNtCore();
            m_ntCore.UpdateRate = 0.5;
            m_ntCore.RemoteName = ServerRemoteName;
            m_ntCore.StartServer(PersistentFilename, "", StandaloneNtCore.DefaultPort);
            m_rpc = new StandaloneRemoteProcedureCall(m_ntCore);
            m_networkTableRoot = new StandaloneNetworkTable(m_ntCore, RootTableName);

            m_networkTableUpdaters.Add(new DriverStationUpdater(m_networkTableRoot,m_rpc, driverStationManager));
            m_networkTableUpdaters.Add(new MatchTimingUpdater(m_networkTableRoot, m_rpc, matchTimingManager));

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

            m_ntCore.Dispose();
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
