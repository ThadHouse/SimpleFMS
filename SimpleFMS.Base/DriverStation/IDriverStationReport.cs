namespace SimpleFMS.Base.DriverStation
{
    /// <summary>
    /// Interface containing a report of the current driver station status
    /// </summary>
    public interface IDriverStationReport
    {
        /// <summary>
        /// Gets the team number
        /// </summary>
        int TeamNumber { get; }
        /// <summary>
        /// Gets the alliance station
        /// </summary>
        AllianceStation Station { get; }
        /// <summary>
        /// Gets if the driver station is connected
        /// </summary>
        bool DriverStationConnected { get; }
        /// <summary>
        /// Gets if the RoboRio is connected
        /// </summary>
        bool RoboRioConnected { get; }
        /// <summary>
        /// Gets if the driver station is being sent enabled
        /// </summary>
        bool IsBeingSentEnabled { get; }
        /// <summary>
        /// Gets if the driver station is being sent autonomous
        /// </summary>
        bool IsBeingSentAutonomous { get; }
        /// <summary>
        /// Gets if the driver station is being sent eStopped
        /// </summary>
        bool IsBeingSentEStopped { get; }
        /// <summary>
        /// Gets if the driver station is receiving enabled from the robot
        /// </summary>
        bool IsReceivingEnabled { get; }
        /// <summary>
        /// Gets if the driver station is receiving autonomous from the robot
        /// </summary>
        bool IsReceivingAutonomous { get; }
        /// <summary>
        /// Gets if the driver station is receiving eStopped from the robot
        /// </summary>
        bool IsReceivingEStopped { get; }
        /// <summary>
        /// Gets if the driver station is bypassed
        /// </summary>
        bool IsBypassed { get; }
        /// <summary>
        /// Gets the current robot battery voltage
        /// </summary>
        double RobotBattery { get; }
    }
}
