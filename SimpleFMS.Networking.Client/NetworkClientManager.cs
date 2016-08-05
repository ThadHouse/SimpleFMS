using System.Collections.Generic;
using NetworkTables;
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

        private readonly object m_lockObject = new object();

        public NetworkClientManager(string clientName)
        {
            m_ntCore = new StandaloneNtCore();
            m_ntCore.RemoteName = clientName;
            m_ntCore.StartClient(FmsIpAddress, NetworkTablesPort);
            m_rpc = new StandaloneRemoteProcedureCall(m_ntCore);
            m_networkTableRoot = new StandaloneNetworkTable(m_ntCore, RootTableName);

            
        }

        public StandaloneNtCore NtCore => m_ntCore;
        public StandaloneNetworkTable NetworkTable => m_networkTableRoot;
        public StandaloneRemoteProcedureCall Rpc => m_rpc;

        public void AddClient(INetworkClient client)
        {
            m_networkTableReceivers.Add(client);
        }

        public void Dispose()
        {
            m_ntCore.Dispose();
        }
    }
}
