using System;
using System.Collections.Generic;
using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Networking.Base;
using static SimpleFMS.Base.Networking.NetworkingConstants;

namespace SimpleFMS.Networking.Client
{
    public class NetworkClientManager : INetworkClientManager
    {
        private readonly List<INetworkClient> m_networkTableReceivers = new List<INetworkClient>();

        private readonly StandaloneNetworkTable m_networkTableRoot;
        private readonly StandaloneNtCore m_ntCore;
        private readonly StandaloneRemoteProcedureCall m_rpc;

        public bool FmsConnected => m_networkTableRoot.IsConnected;

        public NetworkClientManager(string clientName)
        {
            m_ntCore = new StandaloneNtCore();
            m_ntCore.RemoteName = clientName;
            m_ntCore.StartClient(FmsIpAddress, NetworkTablesPort);
            m_rpc = new StandaloneRemoteProcedureCall(m_ntCore);
            m_networkTableRoot = new StandaloneNetworkTable(m_ntCore, RootTableName);

            Action<IRemote, ConnectionInfo, bool> connectionChanged = (remote, info, conn) =>
            {
                if (info.RemoteId == FmsServerRemoteName)
                {
                    OnFmsConnectionChanged?.Invoke(conn);
                }
            };
            m_networkTableRoot.AddConnectionListener(connectionChanged, true);
            m_fmsConnectionListeners.Add(connectionChanged);
        }

        private readonly List<Action<IRemote, ConnectionInfo, bool>> m_fmsConnectionListeners = new List<Action<IRemote, ConnectionInfo, bool>>();

        public void SuspendNetworking()
        {
            m_ntCore.StopClient();
        }

        public void ResumeNetworking()
        {
            if (!m_ntCore.Running)
                m_ntCore.StartClient(FmsIpAddress, NetworkTablesPort);
        }

        public event Action<bool> OnFmsConnectionChanged;

        public StandaloneNtCore NtCore => m_ntCore;
        public StandaloneNetworkTable NetworkTable => m_networkTableRoot;
        public StandaloneRemoteProcedureCall Rpc => m_rpc;

        public void AddClient(INetworkClient client)
        {
            m_networkTableReceivers.Add(client);
        }

        public void Dispose()
        {
            foreach (var fmsConnectionListener in m_fmsConnectionListeners)
            {
                m_networkTableRoot.RemoveConnectionListener(fmsConnectionListener);
            }

            m_ntCore.Dispose();
        }
    }
}
