using System;
using NetworkTables;

namespace SimpleFMS.Networking.Base
{
    public interface INetworkClientManager : IDisposable
    {
        bool FmsConnected { get; }

        void AddClient(INetworkClient client);

        void SuspendNetworking();
        void ResumeNetworking();

        event Action<bool> OnFmsConnectionChanged;

        StandaloneNtCore NtCore { get; }
        StandaloneNetworkTable NetworkTable { get; }
        StandaloneRemoteProcedureCall Rpc { get; }
    }
}
