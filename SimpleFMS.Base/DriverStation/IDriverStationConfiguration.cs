namespace SimpleFMS.Base.DriverStation
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
        /// Gets the alliance station
        /// </summary>
        AllianceStation Station { get; }

        /// <summary>
        /// Gets if the station is bypassed
        /// </summary>
        bool IsBypassed { get; }
    }
}
