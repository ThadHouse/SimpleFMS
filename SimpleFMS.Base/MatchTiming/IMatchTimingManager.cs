﻿using System;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.Base.MatchTiming
{

    public interface IMatchTimingManager : IDisposable
    {
        event Action<TimeSpan> OnMatchTimerUpdate;
        event Action<MatchState, MatchState> OnMatchPeriodUpdate;

        TimeSpan GetRemainingPeriodTime();
        MatchState GetMatchState();

        void StartMatch();
        void StopCurrentPeriod();
        void StartAutonomous();
        void StartTeleop();

        TimeSpan TeleoperatedTime { get; set; }
        TimeSpan AutonomousTime { get; set; }
        TimeSpan DelayTime { get; set; }
    }
}
