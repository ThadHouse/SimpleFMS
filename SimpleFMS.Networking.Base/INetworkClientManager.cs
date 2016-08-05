using System;
using NetworkTables;

namespace SimpleFMS.Networking.Base
{
    public interface INetworkClientManager : IDisposable
    {
        void AddClient(INetworkClient client);

        StandaloneNtCore NtCore { get; }
        StandaloneNetworkTable NetworkTable { get; }
        StandaloneRemoteProcedureCall Rpc { get; }
    }
}
