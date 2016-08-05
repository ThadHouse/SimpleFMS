using System;

namespace SimpleFMS.Base.MatchTiming
{
    public interface IMatchTimes
    {
        TimeSpan TeleoperatedTime { get; }
        TimeSpan DelayTime { get; }
        TimeSpan AutonomousTime { get; }
    }
}
