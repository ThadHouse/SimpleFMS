using System;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.Base.MatchTiming
{
    public interface IMatchTimingReport : IMatchTimes
    {
        MatchState MatchState { get; }
        TimeSpan RemainingPeriodTime { get; }
    }
}
