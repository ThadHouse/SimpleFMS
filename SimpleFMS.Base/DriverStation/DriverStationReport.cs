using System;

namespace SimpleFMS.Base.DriverStation
{
    public class DriverStationReport: IDriverStationReport, IEquatable<DriverStationReport>
    {
        public bool Equals(DriverStationReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TeamNumber == other.TeamNumber && Station.Equals(other.Station) &&
                   DriverStationConnected == other.DriverStationConnected && RoboRioConnected == other.RoboRioConnected &&
                   IsBeingSentEnabled == other.IsBeingSentEnabled &&
                   IsBeingSentAutonomous == other.IsBeingSentAutonomous &&
                   IsBeingSentEStopped == other.IsBeingSentEStopped && IsReceivingEnabled == other.IsReceivingEnabled &&
                   IsReceivingAutonomous == other.IsReceivingAutonomous &&
                   IsReceivingEStopped == other.IsReceivingEStopped && RobotBattery.Equals(other.RobotBattery);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DriverStationReport) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = TeamNumber;
                hashCode = (hashCode*397) ^ Station.GetHashCode();
                hashCode = (hashCode*397) ^ DriverStationConnected.GetHashCode();
                hashCode = (hashCode*397) ^ RoboRioConnected.GetHashCode();
                hashCode = (hashCode*397) ^ IsBeingSentEnabled.GetHashCode();
                hashCode = (hashCode*397) ^ IsBeingSentAutonomous.GetHashCode();
                hashCode = (hashCode*397) ^ IsBeingSentEStopped.GetHashCode();
                hashCode = (hashCode*397) ^ IsReceivingEnabled.GetHashCode();
                hashCode = (hashCode*397) ^ IsReceivingAutonomous.GetHashCode();
                hashCode = (hashCode*397) ^ IsReceivingEStopped.GetHashCode();
                hashCode = (hashCode*397) ^ RobotBattery.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(DriverStationReport left, DriverStationReport right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DriverStationReport left, DriverStationReport right)
        {
            return !Equals(left, right);
        }

        public int TeamNumber { get; set; }
        public AllianceStation Station { get; set; }
        public bool DriverStationConnected { get; set; }
        public bool RoboRioConnected { get; set; }
        public bool IsBeingSentEnabled { get; set; }
        public bool IsBeingSentAutonomous { get; set; }
        public bool IsBeingSentEStopped { get; set; }
        public bool IsReceivingEnabled { get; set; }
        public bool IsReceivingAutonomous { get; set; }
        public bool IsReceivingEStopped { get; set; }
        public bool IsBypassed { get; set; }
        public double RobotBattery { get; set; }
    }
}
