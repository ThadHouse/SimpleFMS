using System;

namespace SimpleFMS.Base.MatchTiming
{
    /// <summary>
    /// Interface containing the current match times
    /// </summary>
    public interface IMatchTimeReport
    {
        /// <summary>
        /// The time for teleoperated to run for
        /// </summary>
        TimeSpan TeleoperatedTime { get; }
        /// <summary>
        /// The delay time betweeen autonomous and teleoperated
        /// </summary>
        TimeSpan DelayTime { get; }
        /// <summary>
        /// The time for autonomous to run for
        /// </summary>
        TimeSpan AutonomousTime { get; }
    }
}
