using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFMS.Base.MatchTiming
{
    public interface IMatchTimes
    {
        TimeSpan TeleoperatedTime { get; }
        TimeSpan DelayTime { get; }
        TimeSpan AutonomousTime { get; }
    }
}
