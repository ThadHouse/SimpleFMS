using System;

namespace SimpleFMS.Base.DriverStation
{
    /// <summary>
    /// A class used to set configuration data for driver stations
    /// </summary>
    public class DriverStationConfiguration : IDriverStationConfiguration, IEquatable<DriverStationConfiguration>
    {
        public bool Equals(DriverStationConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TeamNumber == other.TeamNumber && Station.Equals(other.Station) && IsBypassed == other.IsBypassed;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DriverStationConfiguration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = TeamNumber;
                hashCode = (hashCode*397) ^ Station.GetHashCode();
                hashCode = (hashCode*397) ^ IsBypassed.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(DriverStationConfiguration left, DriverStationConfiguration right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DriverStationConfiguration left, DriverStationConfiguration right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Gets the team number
        /// </summary>
        public int TeamNumber { get; set; }
        
        public AllianceStation Station { get; set; }

        public bool IsBypassed { get; set; }
    }
}
