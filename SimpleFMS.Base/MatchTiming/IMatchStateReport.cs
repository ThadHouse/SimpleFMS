using System;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.Base.MatchTiming
{
    /// <summary>
    /// Contains a report of all current match timing information
    /// </summary>
    public interface IMatchStateReport : IMatchTimeReport
    {
        /// <summary>
        /// Gets the current match state
        /// </summary>
        MatchState MatchState { get; }
        /// <summary>
        /// Gets the time remaining in the current period
        /// </summary>
        TimeSpan RemainingPeriodTime { get; }
    }
}
