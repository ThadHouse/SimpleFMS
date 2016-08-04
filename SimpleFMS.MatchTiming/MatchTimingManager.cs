using System;
using System.Threading;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.Base.Exceptions;
using SimpleFMS.Base.MatchTiming;

namespace SimpleFMS.MatchTiming
{
    public class MatchTimingManager : IMatchTimingManager
    {
        private const int MatchUpdatePeriod = 250;

        private TimeSpan m_teleoperatedTime = MatchTimingConstants.DefaultTeleoperatedTime;
        private TimeSpan m_autonomousTime = MatchTimingConstants.DefaultAutonomousTime;
        private TimeSpan m_delayTime = MatchTimingConstants.DefaultDelayTime;

        public TimeSpan TeleoperatedTime
        {
            get
            {
                lock (m_lockObject)
                    return m_teleoperatedTime;
            }
            set
            {
                if (m_matchState != MatchState.Stopped)
                    throw new MatchEnabledException("Teleoperated time cannot be set while the match is enabled");
                lock (m_lockObject)
                    m_teleoperatedTime = value;
            }
        }

        public TimeSpan AutonomousTime
        {
            get
            {
                lock (m_lockObject)
                    return m_autonomousTime;
            }
            set
            {
                if (m_matchState != MatchState.Stopped)
                    throw new MatchEnabledException("Autonomous time cannot be set while the match is enabled");
                lock (m_lockObject)
                    m_autonomousTime = value;
            }
        }

        public TimeSpan DelayTime
        {
            get
            {
                lock (m_lockObject)
                    return m_delayTime;
            }
            set
            {
                if (m_matchState != MatchState.Stopped)
                    throw new MatchEnabledException("Delay time cannot be set while the match is enabled");
                lock (m_lockObject)
                    m_delayTime = value;
            }
        }

        private MatchState m_matchState = MatchState.Stopped;
        private DateTime m_periodEndTime = DateTime.MinValue;
        private bool m_fullMatch = false;

        private readonly Timer m_matchTimer;

        private readonly IDriverStationManager m_driverStationManager;

        private readonly object m_lockObject = new object();

        public MatchTimingManager(IDriverStationManager dsManager)
        {
            m_driverStationManager = dsManager;

            m_matchTimer = new Timer(OnTimerUpdate);
            m_matchTimer.Change(MatchUpdatePeriod, MatchUpdatePeriod);
        }

        private void OnTimerUpdate(object state)
        {
            lock (m_lockObject)
            {
                if (m_matchState == MatchState.Stopped) return;

                DateTime now = DateTime.UtcNow;
                TimeSpan remaining = m_periodEndTime - now;
                if (remaining <= TimeSpan.Zero)
                {
                    // Period Expired
                    // Stop the current period.
                    m_driverStationManager.StopMatchPeriod();

                    // If we were in autonomous and running a full match
                    if (m_matchState == MatchState.Autonomous && m_fullMatch)
                    {
                        m_matchState = MatchState.Delay;
                        m_periodEndTime = now + DelayTime;
                    }
                    // Finished a pause
                    else if (m_matchState == MatchState.Delay)
                    {
                        m_matchState = MatchState.Teleoperated;
                        m_periodEndTime = now + TeleoperatedTime;
                        m_driverStationManager.StartMatchPertiod(false);
                    }
                    else
                    {
                        // Finished a match
                        OnMatchStop();
                    }
                }
                else
                {
                    m_driverStationManager.SetRemainingMatchTime(remaining.Seconds);
                }
            }
        }

        private void OnMatchStop()
        {
            lock (m_lockObject)
            {
                m_periodEndTime = DateTime.MinValue;
                m_matchState = MatchState.Stopped;
            }
        }

        public void Dispose()
        {
            m_matchTimer.Dispose();
        }

        public event Action<TimeSpan> OnMatchTimerUpdate;
        public event Action<MatchState, MatchState> OnMatchPeriodUpdate;

        public TimeSpan GetRemainingPeriodTime()
        {
            lock (m_lockObject)
            {
                if (m_matchState == MatchState.Stopped)
                    return AutonomousTime;

                if (m_periodEndTime == DateTime.MinValue)
                    return TimeSpan.Zero;

                return m_periodEndTime - DateTime.UtcNow;
            }
        }

        public MatchState GetMatchState()
        {
            lock (m_lockObject)
                return m_matchState;
        }

        public void StartMatch()
        {
            lock (m_lockObject)
            {
                if (m_matchState != MatchState.Stopped) return;

                m_periodEndTime = DateTime.UtcNow + AutonomousTime;
                m_fullMatch = true;
                m_matchState = MatchState.Autonomous;
                m_driverStationManager.StartMatchPertiod(true);
            }
        }

        public void StopCurrentPeriod()
        {
            m_driverStationManager.StopMatchPeriod();
            OnMatchStop();
        }

        public void StartAutonomous()
        {
            lock (m_lockObject)
            {
                if (m_matchState != MatchState.Stopped) return;

                m_matchState = MatchState.Autonomous;
                m_periodEndTime = DateTime.UtcNow + AutonomousTime;
                m_fullMatch = false;
                m_driverStationManager.StartMatchPertiod(true);
            }
        }

        public void StartTeleop()
        {
            lock (m_lockObject)
            {
                if (m_matchState != MatchState.Stopped) return;

                m_matchState = MatchState.Teleoperated;
                m_periodEndTime = DateTime.UtcNow + TeleoperatedTime;
                m_fullMatch = false;
                m_driverStationManager.StartMatchPertiod(false);
            }
        }
    }
}
