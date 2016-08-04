using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.DriverStation.Interfaces;

namespace SimpleFMS.WinForms
{
    /// <summary>
    /// A class used to set configuration data for driver stations
    /// </summary>
    public class DriverStationConfiguration : IDriverStationConfiguration
    {
        /// <summary>
        /// Gets the team number
        /// </summary>
        public int TeamNumber { get; set; }
        
        public AllianceStation Station { get; set; }

        public bool IsBypassed { get; set; }
    }
}
