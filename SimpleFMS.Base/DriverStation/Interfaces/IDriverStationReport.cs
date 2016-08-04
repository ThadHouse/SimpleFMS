namespace SimpleFMS.Base.DriverStation.Interfaces
{
    public interface IDriverStationReport
    {
        int TeamNumber { get; }
        AllianceStation Station { get; }
        bool DriverStationConnected { get; }
        bool RoboRioConnected { get; }
        bool IsBeingSentEnabled { get; }
        bool IsBeingSentAutonomous { get; }
        bool IsBeingSentEStopped { get; }
        bool IsReceivingEnabled { get; }
        bool IsReceivingAutonomous { get; }
        bool IsReceivingEStopped { get; }
        double RobotBattery { get; }
    }
}
