using SimpleFMS.DriverStation.Base.Enums;

namespace SimpleFMS.DriverStation.Base.Interfaces
{
    /// <summary>
    /// Interface for supplying Driver Station Configurations to the 
    /// Driver Station Manager
    /// </summary>
    public interface IDriverStationConfiguration
    {
        /// <summary>
        /// Gets the team number
        /// </summary>
        int TeamNumber { get; }
        /// <summary>
        /// Gets the alliance station side
        /// </summary>
        AllianceStationSide AllianceSide { get; }
        /// <summary>
        /// Gets the alliance station number
        /// </summary>
        AllianceStationNumber StationNumber { get; }

        bool IsBypassed { get; }
    }
}
