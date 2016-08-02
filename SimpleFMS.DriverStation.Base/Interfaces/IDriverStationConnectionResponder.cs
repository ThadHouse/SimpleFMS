using System.Net;

namespace SimpleFMS.DriverStation.Base.Interfaces
{
    public interface IDriverStationConnectionResponder
    {
        void OnDriverStationConnectionChanged(IDriverStationConfiguration configuration, bool connected);
        void OnRobotConnectionChanged(IDriverStationConfiguration configuration, bool connected);
        void AllConnectionReset();
    }
}
