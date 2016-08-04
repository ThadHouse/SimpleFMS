namespace SimpleFMS.Base.DriverStation.Interfaces
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
        
        AllianceStation Station { get; }

        bool IsBypassed { get; }
    }
}
