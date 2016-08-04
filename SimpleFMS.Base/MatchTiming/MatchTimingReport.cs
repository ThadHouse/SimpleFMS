using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.Base.MatchTiming
{
    public class MatchTimingReport : IMatchTimingReport, IEquatable<MatchTimingReport>
    {
        public bool Equals(MatchTimingReport other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MatchState == other.MatchState && RemainingPeriodTime.Equals(other.RemainingPeriodTime) &&
                   TeleoperatedTime.Equals(other.TeleoperatedTime) && DelayTime.Equals(other.DelayTime) &&
                   AutonomousTime.Equals(other.AutonomousTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MatchTimingReport) obj);
        }

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

        public static bool operator ==(MatchTimingReport left, MatchTimingReport right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MatchTimingReport left, MatchTimingReport right)
        {
            return !Equals(left, right);
        }

        public MatchState MatchState { get; set; }
        public TimeSpan RemainingPeriodTime { get; set; }
        public TimeSpan TeleoperatedTime { get; set; }
        public TimeSpan DelayTime { get; set; }
        public TimeSpan AutonomousTime { get; set; }
    }
}
