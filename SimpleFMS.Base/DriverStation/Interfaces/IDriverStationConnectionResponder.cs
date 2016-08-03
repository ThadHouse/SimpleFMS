namespace SimpleFMS.Base.DriverStation.Interfaces
{
    public interface IDriverStationConnectionResponder
    {
        void OnDriverStationConnectionChanged(IDriverStationConfiguration configuration, bool connected);
        void OnRobotConnectionChanged(IDriverStationConfiguration configuration, bool connected);
        void AllConnectionReset();
    }
}
