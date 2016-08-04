using System;

namespace SimpleFMS.Base.MatchTiming
{
    public static class MatchTimingConstants
    {
        public static readonly TimeSpan DefaultTeleoperatedTime = TimeSpan.FromSeconds(135);
        public static readonly TimeSpan DefaultAutonomousTime = TimeSpan.FromSeconds(15);
        public static readonly TimeSpan DefaultDelayTime = TimeSpan.FromSeconds(1);
    }
}
