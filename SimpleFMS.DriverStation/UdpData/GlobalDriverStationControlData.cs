using System;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.DriverStation.UdpData
{
    /// <summary>
    /// Holds driver station data global to all connected
    /// driver stations
    /// </summary>
    public static class GlobalDriverStationControlData
    {
        /// <summary>
        /// The current match type being ran
        /// </summary>
        public static MatchType MatchType { get; set; }
        
        /// <summary>
        /// The current match number being ran
        /// </summary>
        public static int MatchNumber { get; set; }

        /// <summary>
        /// The local FMS time
        /// </summary>
        public static DateTime FmsTime { get; set; }

        /// <summary>
        /// The time remaining in the current match period
        /// </summary>
        public static int MatchTimeRemaining { get; set; }

        /// <summary>
        /// If the match is in autonomous mode
        /// </summary>
        public static bool IsAutonomous { get; set; }

        /// <summary>
        /// If the match is currently enabled
        /// </summary>
        public static bool IsEnabled { get; set; }

    }
}
