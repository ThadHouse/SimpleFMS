using System;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.Base.MatchTiming
{
    /// <summary>
    /// Interface for handling all match timing
    /// </summary>
    public interface IMatchTimingManager : IDisposable
    {
        /// <summary>
        /// Occurs when the match timer is updated
        /// </summary>
        event Action<TimeSpan> OnMatchTimerUpdate;
        /// <summary>
        /// Occurs when the match period is changed
        /// </summary>
        event Action<MatchState, MatchState> OnMatchPeriodUpdate;

        /// <summary>
        /// Gets the remaining time left in the period
        /// </summary>
        /// <returns>The time left in the period</returns>
        TimeSpan GetRemainingPeriodTime();
        /// <summary>
        /// Gets the current state of the match
        /// </summary>
        /// <returns>The state of the match</returns>
        MatchState GetMatchState();

        /// <summary>
        /// Starts a match
        /// </summary>
        void StartMatch();
        /// <summary>
        /// Stops any current running period
        /// </summary>
        void StopCurrentPeriod();
        /// <summary>
        /// Starts autonomous mode only
        /// </summary>
        void StartAutonomous();
        /// <summary>
        /// Starts teleoperated mode only
        /// </summary>
        void StartTeleop();

        /// <summary>
        /// Sets the times the match should run for
        /// </summary>
        /// <param name="times">The times the match should run for</param>
        /// <returns>True if the times were set correctly</returns>
        bool SetMatchTimes(IMatchTimeReport times);

        /// <summary>
        /// Get a report containing all of the current match information
        /// </summary>
        /// <returns>The match report</returns>
        IMatchStateReport GetMatchTimingReport();
    }
}
