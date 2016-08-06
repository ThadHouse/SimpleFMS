using System;
using NetworkTables;

namespace SimpleFMS.Networking.Base
{
    public interface INetworkClientManager : IDisposable
    {
        void AddClient(INetworkClient client);

        event Action<bool> OnFmsConnectionChanged;

        StandaloneNtCore NtCore { get; }
        StandaloneNetworkTable NetworkTable { get; }
        StandaloneRemoteProcedureCall Rpc { get; }
    }
}
