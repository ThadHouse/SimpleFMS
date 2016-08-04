using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
