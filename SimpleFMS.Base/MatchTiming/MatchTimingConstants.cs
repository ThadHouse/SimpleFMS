using System;

namespace SimpleFMS.Base.MatchTiming
{
    /// <summary>
    /// Contstants related to match timing
    /// </summary>
    public static class MatchTimingConstants
    {
        /// <summary>
        /// Gets the default teloperated period time
        /// </summary>
        public static readonly TimeSpan DefaultTeleoperatedTime = TimeSpan.FromSeconds(135);
        /// <summary>
        /// Gets the default autonomous period time
        /// </summary>
        public static readonly TimeSpan DefaultAutonomousTime = TimeSpan.FromSeconds(15);
        /// <summary>
        /// Gets the default delay time between the autonomous and teleoperated periods
        /// </summary>
        public static readonly TimeSpan DefaultDelayTime = TimeSpan.FromSeconds(1);
    }
}
