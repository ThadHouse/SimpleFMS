using System;

namespace SimpleFMS.Base.MatchTiming
{
    /// <summary>
    /// Structure containing the match times
    /// </summary>
    public struct MatchTimeReport : IMatchTimeReport, IEquatable<MatchTimeReport>
    {
        /// <summary>
        /// Create a new Match Time Report
        /// </summary>
        /// <param name="teleoperatedTime">The teleoperated period time</param>
        /// <param name="delayTime">The delay time between autonomous and teleoperated</param>
        /// <param name="autonomousTime">The autonomous period time</param>
        public MatchTimeReport(TimeSpan teleoperatedTime, TimeSpan delayTime, TimeSpan autonomousTime)
        {
            TeleoperatedTime = teleoperatedTime;
            DelayTime = delayTime;
            AutonomousTime = autonomousTime;
        }

        /// <inheritdoc/>
        public bool Equals(MatchTimeReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TeleoperatedTime.Equals(other.TeleoperatedTime) && DelayTime.Equals(other.DelayTime) &&
                   AutonomousTime.Equals(other.AutonomousTime);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MatchTimeReport) obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = TeleoperatedTime.GetHashCode();
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
        public static bool operator ==(MatchTimeReport left, MatchTimeReport right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Gets if these 2 instances are not equal
        /// </summary>
        /// <param name="left">The left operator</param>
        /// <param name="right">The right operator</param>
        /// <returns>True if the values are not equal</returns>
        public static bool operator !=(MatchTimeReport left, MatchTimeReport right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc/>
        public TimeSpan TeleoperatedTime { get; }
        /// <inheritdoc/>
        public TimeSpan DelayTime { get; }
        /// <inheritdoc/>
        public TimeSpan AutonomousTime { get; }
    }
}
