using System;
using System.Net;

namespace SimpleFMS.Base.Networking
{
    public interface INetworkClientManager : IDisposable
    {
        void AddClient(INetworkClient client);
    }
}
