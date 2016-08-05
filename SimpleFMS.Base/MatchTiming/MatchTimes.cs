using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFMS.Base.MatchTiming
{
    public class MatchTimes : IMatchTimes, IEquatable<MatchTimes>
    {
        public bool Equals(MatchTimes other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TeleoperatedTime.Equals(other.TeleoperatedTime) && DelayTime.Equals(other.DelayTime) &&
                   AutonomousTime.Equals(other.AutonomousTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MatchTimes) obj);
        }

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

        public static bool operator ==(MatchTimes left, MatchTimes right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MatchTimes left, MatchTimes right)
        {
            return !Equals(left, right);
        }

        public TimeSpan TeleoperatedTime { get; set; }
        public TimeSpan DelayTime { get; set; }
        public TimeSpan AutonomousTime { get; set; }
    }
}
