namespace SimpleFMS.Base.DriverStation.Interfaces
{
    /// <summary>
    /// Data associated with an incoming data connection from a Driver Station
    /// </summary>
    public interface IDriverStationIncomingData
    {
        bool HasDriverStationComms { get; }

        bool HasRobotComms { get; }

        bool IsEStopped { get; }

        bool IsEnabled { get; }

        bool IsAutonomous { get; }

        int TeamNumber { get; }

        double RobotBattery { get; }
    }
}
