namespace SimpleFMS.Base.Enums
{
    /// <summary>
    /// All of the valid match states
    /// </summary>
    public enum MatchState
    {
        /// <summary>
        /// The match is stopped, and waiting to be started
        /// </summary>
        Stopped,
        /// <summary>
        /// The match is in autonomous mode
        /// </summary>
        Autonomous,
        /// <summary>
        ///  The match is in delay mode between autonomous and teleoperated
        /// </summary>
        Delay,
        /// <summary>
        /// The match is in teleoperated mode
        /// </summary>
        Teleoperated
    }
}
