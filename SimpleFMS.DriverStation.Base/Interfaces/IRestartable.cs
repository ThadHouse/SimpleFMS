using System;

namespace SimpleFMS.DriverStation.Base.Interfaces
{
    /// <summary>
    /// Interface stating a service can be externally restarted and disposed
    /// </summary>
    public interface IRestartable : IDisposable
    {
        /// <summary>
        /// Restarts this service
        /// </summary>
        void Restart();
    }
}
