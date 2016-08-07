using System;

namespace SimpleFMS.Base.Networking
{
    public interface INetworkServerManager : IDisposable
    {
        event Action<string, string, bool> OnClientChanged;
    }
}
