namespace SimpleFMS.Base.Enums
{
    /// <summary>
    /// The connection of a driver station
    /// </summary>
    public enum AllianceStationStatus
    {
        /// <summary>
        /// The driver station is plugged into the correct station
        /// </summary>
        CorrectStation,
        /// <summary>
        /// The driver station is the correct team, but plugged into
        /// the wrong station
        /// </summary>
        IncorrectStation,
        /// <summary>
        /// The driver station is a team nut in the current match
        /// </summary>
        InvalidTeam
    }
}
