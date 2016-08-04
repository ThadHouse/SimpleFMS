using SimpleFMS.Base.DriverStation;

namespace SimpleFMS.DriverStation
{
    internal class DriverStationReport: IDriverStationReport
    {
        public int TeamNumber { get; internal set;}
        public AllianceStation Station { get; internal set;}
        public bool DriverStationConnected { get; internal set;}
        public bool RoboRioConnected { get; internal set;}
        public bool IsBeingSentEnabled { get; internal set;}
        public bool IsBeingSentAutonomous { get; internal set;}
        public bool IsBeingSentEStopped { get; internal set;}
        public bool IsReceivingEnabled { get; internal set;}
        public bool IsReceivingAutonomous { get; internal set;}
        public bool IsReceivingEStopped { get; internal set;}
        public double RobotBattery { get; internal set;}
    }
}
