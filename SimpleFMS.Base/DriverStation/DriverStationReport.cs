using System;

namespace SimpleFMS.Base.DriverStation
{
    /// <summary>
    /// A class used to hold report data for each driver station
    /// </summary>
    public struct DriverStationReport: IDriverStationReport, IEquatable<DriverStationReport>
    {
        /// <summary>
        /// Creates a new Driver Station Report
        /// </summary>
        /// <param name="teamNumber">The team number</param>
        /// <param name="station">The alliance station</param>
        /// <param name="driverStationConnected">True if the ds is connected</param>
        /// <param name="roboRioConnected">True if the rio is connected</param>
        /// <param name="isBeingSentEnabled">True if the ds is being sent enabled</param>
        /// <param name="isBeingSentAutonomous">True if the ds is being sent autonomous</param>
        /// <param name="isBeingSentEStopped">True if the ds is being sent estopped</param>
        /// <param name="isReceivingEnabled">True if receiving enabled from the ds</param>
        /// <param name="isReceivingAutonomous">True if receiving autonomous from the ds</param>
        /// <param name="isReceivingEStopped">True if receiving estopped from the ds</param>
        /// <param name="isBypassed">True if the ds is bypassed</param>
        /// <param name="robotBattery">The robot battery voltage</param>
        public DriverStationReport(int teamNumber, AllianceStation station, bool driverStationConnected, bool roboRioConnected, bool isBeingSentEnabled, bool isBeingSentAutonomous, bool isBeingSentEStopped, bool isReceivingEnabled, bool isReceivingAutonomous, bool isReceivingEStopped, bool isBypassed, double robotBattery)
        {
            TeamNumber = teamNumber;
            Station = station;
            DriverStationConnected = driverStationConnected;
            RoboRioConnected = roboRioConnected;
            IsBeingSentEnabled = isBeingSentEnabled;
            IsBeingSentAutonomous = isBeingSentAutonomous;
            IsBeingSentEStopped = isBeingSentEStopped;
            IsReceivingEnabled = isReceivingEnabled;
            IsReceivingAutonomous = isReceivingAutonomous;
            IsReceivingEStopped = isReceivingEStopped;
            IsBypassed = isBypassed;
            RobotBattery = robotBattery;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DriverStationReport) obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = TeamNumber;
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

        /// <summary>
        /// Checks to see if 2 <see cref="DriverStationReport">Driver Station Reports</see>
        /// are equal
        /// </summary>
        /// <param name="left">The left operator</param>
        /// <param name="right">The right operator</param>
        /// <returns>True if the reports are equal</returns>
        public static bool operator ==(DriverStationReport left, DriverStationReport right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Checks to see if 2 <see cref="DriverStationReport">Driver Station Reports</see>
        /// are not equal
        /// </summary>
        /// <param name="left">The left operator</param>
        /// <param name="right">The right operator</param>
        /// <returns>True if the reports are not equal</returns>
        public static bool operator !=(DriverStationReport left, DriverStationReport right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc/>
        public int TeamNumber { get; }
        /// <inheritdoc/>
        public AllianceStation Station { get; }
        /// <inheritdoc/>
        public bool DriverStationConnected { get; }
        /// <inheritdoc/>
        public bool RoboRioConnected { get;  }
        /// <inheritdoc/>
        public bool IsBeingSentEnabled { get;  }
        /// <inheritdoc/>
        public bool IsBeingSentAutonomous { get;  }
        /// <inheritdoc/>
        public bool IsBeingSentEStopped { get;  }
        /// <inheritdoc/>
        public bool IsReceivingEnabled { get;  }
        /// <inheritdoc/>
        public bool IsReceivingAutonomous { get; }
        /// <inheritdoc/>
        public bool IsReceivingEStopped { get; }
        /// <inheritdoc/>
        public bool IsBypassed { get; }
        /// <inheritdoc/>
        public double RobotBattery { get;  }
    }
}
