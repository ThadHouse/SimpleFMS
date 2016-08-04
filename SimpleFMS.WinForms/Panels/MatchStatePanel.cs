using System;
using System.Drawing;
using System.Windows.Forms;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.Base.MatchTiming;
using SimpleFMS.MatchTiming;

namespace SimpleFMS.WinForms.Panels
{
    public class MatchStatePanel : Panel
    {
        enum CurrentState
        {
            MatchSimulation,
            TeleoperatedOnly,
            AutonomousOnly
        }

        private CurrentState m_currentState = CurrentState.MatchSimulation;
        //private RunState m_runState = RunState.Stopped;

        private Button m_matchStateButton;
        private TextBox m_teleopTime;
        private TextBox m_autonTime;
        private TextBox m_periodGap;
        private Label m_matchTimer;


        private Timer m_updateTimer;
        private IMatchTimingManager m_matchManager;

        public MatchStatePanel(IMatchTimingManager matchTimingManger)
        {
            m_matchManager = matchTimingManger;

            m_updateTimer = new Timer();
            m_updateTimer.Interval = 500;
            m_updateTimer.Tick += UpdateTimerOnTick;
            m_updateTimer.Start();
            


            //m_timer = new Timer(OnPeriodTimerElapsed);
            //m_timer.Change(500, 500);

            
            Size = new Size(400, 400);

            m_matchTimer = new Label()
            {
                Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Text = "Time Remaining: 15",
                Size = new Size(300, 31),
                Location = new Point(3, 3)
            };
            Controls.Add(m_matchTimer);

            m_matchStateButton = new Button()
            {
                Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Text = "Start FMS Match",
                Enabled = false,
                Size = new Size(m_matchTimer.Size.Width, 40),
                Location = new Point(m_matchTimer.Location.X, m_matchTimer.Location.Y + m_matchTimer.Size.Height + 5)
            };
            Controls.Add(m_matchStateButton);

            Label label = new Label
            {
                AutoSize = true,
                Text = "Teleop Time",
                Location = new Point(m_matchTimer.Location.X + 15, m_matchStateButton.Location.Y + m_matchStateButton.Size.Height + 5),
            };
            Controls.Add(label);

            label = new Label
            {
                AutoSize = true,
                Text = "Autonomous Time",
                Location = new Point(m_matchTimer.Location.X + 95, m_matchStateButton.Location.Y + m_matchStateButton.Size.Height + 5),
            };
            Controls.Add(label);

            m_teleopTime = new TextBox
            {
                Text = "135",
                Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Size = new Size(70, 38),
                Location = new Point(m_matchTimer.Location.X + 15, label.Location.Y + label.Size.Height + 5)
            };
            Controls.Add(m_teleopTime);

            m_autonTime = new TextBox
            {
                Text = "15",
                Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Size = new Size(70, 38),
                Location = new Point(m_teleopTime.Location.X + m_teleopTime.Size.Width + 20, label.Location.Y + label.Size.Height + 5)
            };
            Controls.Add(m_autonTime);

            m_periodGap = new TextBox
            {
                Text = "3",
                Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Size = new Size(70, 38),
                Location = new Point(m_autonTime.Location.X + m_autonTime.Size.Width + 20, m_autonTime.Location.Y)
            };
            Controls.Add(m_periodGap);

            int oldLabelHeight = label.Size.Height;
            label = new Label
            {
                AutoSize = true,
                Text = "Period Delay",
                Location = new Point(m_periodGap.Location.X, m_periodGap.Location.Y - oldLabelHeight - 5)
            };
            Controls.Add(label);

            m_matchTimer.DoubleClick += M_matchTimer_DoubleClick;
            m_autonTime.DoubleClick += M_autonTime_DoubleClick;
            m_teleopTime.DoubleClick += M_teleopTime_DoubleClick;
            m_matchStateButton.Click += M_matchStateButton_Click;
        }

        private void UpdateTimerOnTick(object sender, EventArgs eventArgs)
        {
            TimeSpan time = m_matchManager.GetRemainingPeriodTime();
            UpdateMatchTimer(time.Seconds + time.Minutes * 60);
        }

        private void M_matchStateButton_Click(object sender, EventArgs e)
        {
            if (!m_matchStateButton.Enabled) return;

            if (m_matchManager.GetMatchState() != MatchState.Stopped)
            {
                // Stop the match
                OnDisable();
                return;
            }

            int telopTime;
            TimeSpan teleopTimeSpan = TimeSpan.Zero;
            if (int.TryParse(m_teleopTime.Text, out telopTime))
            {
                teleopTimeSpan = TimeSpan.FromSeconds(telopTime > 0 ? telopTime : 135);
            }

            int autoTime;
            TimeSpan autoTimeSpan = TimeSpan.Zero;
            if (int.TryParse(m_autonTime.Text, out autoTime))
            {
                autoTimeSpan = TimeSpan.FromSeconds(autoTime > 0 ? autoTime : 15);
            }

            int delayTime;
            TimeSpan delayTimeSpan = TimeSpan.Zero;
            if (int.TryParse(m_periodGap.Text, out delayTime))
            {
                delayTimeSpan = TimeSpan.FromSeconds(delayTime > 0 ? delayTime : 2);
            }

            DateTime now = DateTime.UtcNow;
            //m_periodEndTime = DateTime.MinValue;

            switch (m_currentState)
            {
                case CurrentState.MatchSimulation:
                    m_matchManager.AutonomousTime = autoTimeSpan;
                    m_matchManager.TeleoperatedTime = teleopTimeSpan;
                    m_matchManager.DelayTime = delayTimeSpan;
                    m_matchManager.StartMatch();
                    m_matchStateButton.Text = "Stop Autonomous";
                    //m_runState = RunState.Autonomous;
                    break;
                case CurrentState.TeleoperatedOnly:
                    m_matchManager.StartTeleop();
                    m_matchStateButton.Text = "Stop Teleoperated";
                    //m_runState = RunState.Teleoperated;
                    break;
                case CurrentState.AutonomousOnly:
                    m_matchManager.StartAutonomous();
                    m_matchStateButton.Text = "Stop Autonomous";
                    //m_runState = RunState.Autonomous;
                    break;
            }
        }

        public void EnableStart()
        {
            m_matchStateButton.Enabled = true;
        }

        public void UpdateMatchTimer(int time)
        {
            m_matchTimer.Text = $"Time Remaining: {time}";
        }

        private void M_teleopTime_DoubleClick(object sender, EventArgs e)
        {
            if (m_matchManager.GetMatchState() != MatchState.Stopped) return;
            m_currentState = CurrentState.TeleoperatedOnly;
            m_matchStateButton.Text = "Start Teleop Only";
            int time = 0;
            if (int.TryParse(m_teleopTime.Text, out time))
            {
                UpdateMatchTimer(time);
            }
        }

        private void M_autonTime_DoubleClick(object sender, EventArgs e)
        {
            if (m_matchManager.GetMatchState() != MatchState.Stopped) return;
            m_currentState = CurrentState.AutonomousOnly;
            m_matchStateButton.Text = "Start Autonomous Only";
            int time = 0;
            if (int.TryParse(m_autonTime.Text, out time))
            {
                UpdateMatchTimer(time);
            }
        }

        private void M_matchTimer_DoubleClick(object sender, EventArgs e)
        {
            if (m_matchManager.GetMatchState() != MatchState.Stopped) return;
            m_currentState = CurrentState.TeleoperatedOnly;
            m_matchStateButton.Text = "Start FMS Match";
            int time = 0;
            if (int.TryParse(m_autonTime.Text, out time))
            {
                UpdateMatchTimer(time);
            }
        }

        private void OnDisable()
        {
            m_matchManager.StopCurrentPeriod();

            switch (m_currentState)
            {
                case CurrentState.MatchSimulation:
                    m_matchStateButton.Text = "Start FMS Match";
                    break;
                case CurrentState.TeleoperatedOnly:
                    m_matchStateButton.Text = "Start Teleoperated Only";
                    break;
                case CurrentState.AutonomousOnly:
                    m_matchStateButton.Text = "Start Autonomous Only";
                    break;
            }
        }

        /*
        private void OnPeriodTimerElapsed(object s)
        {
            // Enable Teleop
            Invoke((MethodInvoker)delegate
           {
               if (m_periodEndTime == DateTime.MinValue) return;

               DateTime now = DateTime.UtcNow;
               TimeSpan remaining = m_periodEndTime - now;
               if (remaining <= TimeSpan.Zero)
               {
                        // Period Expired
                        // Stop the current period.
                        m_dsManager.StopMatchPeriod();

                        // If was auton, and is simulating a match
                        if (m_runState == RunState.Autonomous && m_currentState == CurrentState.MatchSimulation)
                   {
                       m_matchStateButton.Text = "Stop Period Pause";
                       m_runState = RunState.Pause;
                       int pauseTime;
                       TimeSpan pauseTimeSpan = TimeSpan.FromSeconds(3);
                       if (int.TryParse(m_periodGap.Text, out pauseTime))
                       {
                           pauseTimeSpan = TimeSpan.FromSeconds(pauseTime > 0 ? pauseTime : 3);
                       }

                       m_periodEndTime = now + pauseTimeSpan;
                   }
                        // Finished a pause
                        else if (m_runState == RunState.Pause)
                   {
                       m_matchStateButton.Text = "Stop Teleoperated";
                       m_runState = RunState.Teleoperated;
                       int telopTime;
                       TimeSpan teleopTimeSpan = TimeSpan.Zero;
                       if (int.TryParse(m_teleopTime.Text, out telopTime))
                       {
                           teleopTimeSpan = TimeSpan.FromSeconds(telopTime > 0 ? telopTime : 135);
                       }

                       m_periodEndTime = now + teleopTimeSpan;
                       m_dsManager.StartMatchPertiod(false);
                   }
                   else
                   {
                       OnDisable();
                   }
               }
               else
               {
                   m_dsManager.SetRemainingMatchTime(remaining.Seconds);
                   UpdateMatchTimer(remaining.Seconds);
               }

           });
        }
        */
    }
}
