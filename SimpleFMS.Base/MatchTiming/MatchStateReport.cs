using System;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.Base.MatchTiming
{
    /// <summary>
    /// Contains a report of the current match state
    /// </summary>
    public struct MatchStateReport : IMatchStateReport, IEquatable<MatchStateReport>
    {
        /// <summary>
        /// Creates a new Match State Report
        /// </summary>
        /// <param name="matchState">The current state</param>
        /// <param name="remainingPeriodTime">The remaining period time</param>
        /// <param name="teleoperatedTime">The teleoperated time period</param>
        /// <param name="delayTime">The delay time period</param>
        /// <param name="autonomousTime">The autonomous time period</param>
        public MatchStateReport(MatchState matchState, TimeSpan remainingPeriodTime, TimeSpan teleoperatedTime, TimeSpan delayTime, TimeSpan autonomousTime)
        {
            MatchState = matchState;
            RemainingPeriodTime = remainingPeriodTime;
            TeleoperatedTime = teleoperatedTime;
            DelayTime = delayTime;
            AutonomousTime = autonomousTime;
        }

        /// <inheritdoc/>
        public bool Equals(MatchStateReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MatchState == other.MatchState && RemainingPeriodTime.Equals(other.RemainingPeriodTime) &&
                   TeleoperatedTime.Equals(other.TeleoperatedTime) && DelayTime.Equals(other.DelayTime) &&
                   AutonomousTime.Equals(other.AutonomousTime);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MatchStateReport) obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) MatchState;
                hashCode = (hashCode*397) ^ RemainingPeriodTime.GetHashCode();
                hashCode = (hashCode*397) ^ TeleoperatedTime.GetHashCode();
                hashCode = (hashCode*397) ^ DelayTime.GetHashCode();
                hashCode = (hashCode*397) ^ AutonomousTime.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Gets if these 2 instances are equal
        /// </summary>
        /// <param name="left">The left operator</param>
        /// <param name="right">The right operator</param>
        /// <returns>True if the values are equal</returns>
        public static bool operator ==(MatchStateReport left, MatchStateReport right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Gets if these 2 instances are not equal
        /// </summary>
        /// <param name="left">The left operator</param>
        /// <param name="right">The right operator</param>
        /// <returns>True if the values are not equal</returns>
        public static bool operator !=(MatchStateReport left, MatchStateReport right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc/>
        public MatchState MatchState { get; }
        /// <inheritdoc/>
        public TimeSpan RemainingPeriodTime { get; }
        /// <inheritdoc/>
        public TimeSpan TeleoperatedTime { get; }
        /// <inheritdoc/>
        public TimeSpan DelayTime { get; }
        /// <inheritdoc/>
        public TimeSpan AutonomousTime { get; }
    }
}
