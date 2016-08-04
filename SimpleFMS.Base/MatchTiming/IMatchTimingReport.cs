using System;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.Base.MatchTiming
{
    public interface IMatchTimingReport
    {
        MatchState MatchState { get; }
        TimeSpan RemainingPeriodTime { get; }
        TimeSpan TeleoperatedTime { get; }
        TimeSpan DelayTime { get; }
        TimeSpan AutonomousTime { get; }
    }
}
