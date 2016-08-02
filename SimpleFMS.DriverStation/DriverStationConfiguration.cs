using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleFMS.DriverStation.Base.Enums;
using SimpleFMS.DriverStation.Base.Interfaces;

namespace SimpleFMS.DriverStation
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
        /// <summary>
        /// Gets the alliance station side
        /// </summary>
        public AllianceStationSide AllianceSide { get; set; }
        /// <summary>
        /// Gets the alliance station number
        /// </summary>
        public AllianceStationNumber StationNumber { get; set; }

        public bool IsBypassed { get; set; }
    }
}
