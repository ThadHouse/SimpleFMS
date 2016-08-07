using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Autofac;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Networking.Base;
using SimpleFMS.Networking.Client;
using SimpleFMS.Networking.Client.NetworkClients;

namespace SimpleFMS.Android
{
    public class RetaintedDeviceState : Fragment
    {
        public IContainer AutoFacContainer { get; }

        

        public RetaintedDeviceState()
        {
            INetworkClientManager networkManager = new NetworkClientManager("Android FMS Client");
            DriverStationClient dsClient = new DriverStationClient(networkManager);
            MatchTimingClient matchClient = new MatchTimingClient(networkManager);

            networkManager.AddClient(dsClient);
            networkManager.AddClient(matchClient);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(networkManager).As<INetworkClientManager>();
            builder.RegisterInstance(dsClient).As<DriverStationClient>();
            builder.RegisterInstance(matchClient).As<MatchTimingClient>();

            AutoFacContainer = builder.Build();
        }

        private CurrentDeviceState m_state = CurrentDeviceState.Dirty;
        public CurrentDeviceState State
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RetainInstance = true;
        }

        public void StopApplication()
        {
            using (var scope = AutoFacContainer.BeginLifetimeScope())
            {
                var dsClient = scope.Resolve<DriverStationClient>();
                var netManager = scope.Resolve<INetworkClientManager>();

                netManager.OnFmsConnectionChanged -= m_fmsConnectionAction;
                dsClient.OnDriverStationReportsChanged -= m_dsReportAction;


                netManager.SuspendNetworking();
            }
        }

        public void StartApplication()
        {
            using (var scope = AutoFacContainer.BeginLifetimeScope())
            {
                var dsClient = scope.Resolve<DriverStationClient>();
                var netManager = scope.Resolve<INetworkClientManager>();

                netManager.ResumeNetworking();
                netManager.OnFmsConnectionChanged += m_fmsConnectionAction;
                dsClient.OnDriverStationReportsChanged += m_dsReportAction;
            }
        }

        public void CreateApplication(Action<IReadOnlyDictionary<AllianceStation, IDriverStationReport>> dsAction, Action<bool> fmsAction)
        {
                m_fmsConnectionAction = fmsAction;
                m_dsReportAction = dsAction;
        }

        private Action<IReadOnlyDictionary<AllianceStation, IDriverStationReport>> m_dsReportAction;
        private Action<bool> m_fmsConnectionAction;
    }

    public enum CurrentDeviceState
    {
        Dirty,
        ReadyToRun,
        Running
    }

    [Activity(Label = "Simple FMS Android Controller", MainLauncher = true)]
    public class BaseFMSLayout : Activity
    {
        private readonly Dictionary<AllianceStation, FmsAllianceStation> m_fmsAllianceStations =
            new Dictionary<AllianceStation, FmsAllianceStation>(AllianceStation.MaxNumAllianceStations);

        private MatchTiming m_matchTiming;
        private Button m_initializeMatchButton;
        private EditText m_matchNumberView;


        private RetaintedDeviceState m_retainedState;

        public CurrentDeviceState CurrentState
        {
            get { return m_retainedState.State; }
            set { m_retainedState.State = value; }
        }

        protected override void OnStart()
        {
            base.OnStart();

            m_retainedState.StartApplication();
        }

        protected override void OnStop()
        {
            base.OnStop();

            m_retainedState.StopApplication();
        }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.Window.SetSoftInputMode(SoftInput.StateHidden);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainFmsScreen);

            FragmentManager fm = FragmentManager;
            m_retainedState = fm.FindFragmentByTag<RetaintedDeviceState>("data");

            if (m_retainedState == null)
            {
                m_retainedState = new RetaintedDeviceState();
                fm.BeginTransaction().Add(m_retainedState, "data").Commit();
            }

            m_retainedState.CreateApplication(OnDriverStationReportsChanged, OnFmsConnectionChanged);

            var idConstants = GetIdConstants();

            for (int i = 0; i < AllianceStation.MaxNumAllianceStations; i++)
            {
                FmsAllianceStation fmsStation = new FmsAllianceStation(this, i,
                    idConstants, m_retainedState);
                m_fmsAllianceStations.Add(new AllianceStation((byte)i), fmsStation);
            }

            m_initializeMatchButton = FindViewById<Button>(Resource.Id.initializeMatchButton);

            m_initializeMatchButton.Click += OnInitializeMatchButtonClick;

            m_matchNumberView = FindViewById<EditText>(Resource.Id.matchNumberText);

            m_matchNumberView.TextChanged += (sender, args) =>
            {
                SetDirtySettings();
            };

            var matchStatusRelativeLayout = FindViewById<RelativeLayout>(Resource.Id.matchTimeSwitcher);
            var matchRunningTable = FindViewById<TableLayout>(Resource.Id.matchRunningLayout);
            var matchStoppedTable = FindViewById<TableLayout>(Resource.Id.matchStoppedLayout);

            m_matchTiming = new MatchTiming(matchStatusRelativeLayout, matchStoppedTable, matchRunningTable, this, m_retainedState);

            using (var scope = m_retainedState.AutoFacContainer.BeginLifetimeScope())
            {
                var dsClient = scope.Resolve<DriverStationClient>();
                var netManager = scope.Resolve<INetworkClientManager>();

                var dsReport = dsClient.GetDriverStationReports();
                OnDriverStationReportsChanged(dsReport);
                
                OnFmsConnectionChanged(netManager.FmsConnected);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CurrentDeviceState oldState = m_retainedState.State;

            foreach (var fmsAllianceStation in m_fmsAllianceStations)
            {
                fmsAllianceStation.Value.Dispose();
            }
            m_matchTiming.Dispose();

            //m_retainedState.DestroyApplication();

            m_retainedState.State = oldState;
        }

        private void OnDriverStationReportsChanged(IReadOnlyDictionary<AllianceStation, IDriverStationReport> obj)
        {
            if (obj == null) return;
            RunOnUiThread(() =>
            {

                foreach (var s in obj)
                {
                    FmsAllianceStation station = null;
                    if (m_fmsAllianceStations.TryGetValue(s.Key, out station))
                    {
                        // Station exists.
                        station.UpdateStationReport(s.Value);
                    }

                }
            });
        }

        public void SetDirtySettings()
        {
            m_matchTiming.DisableMatchStarting();
            m_retainedState.State = CurrentDeviceState.Dirty;
        }

        public void SetCleanSettings()
        {
            m_matchTiming.EnableMatchStarting();
            m_retainedState.State = CurrentDeviceState.ReadyToRun;
        }

        public void UpdateMatchState(bool running)
        {
            RunOnUiThread(() =>
            {
                foreach (var fmsAllianceStation in m_fmsAllianceStations)
                {
                    fmsAllianceStation.Value.UpdateMatchState(running);
                }

            });
            m_matchNumberView.Enabled = !running;
            m_initializeMatchButton.Enabled = !running;
            m_retainedState.State = running ? CurrentDeviceState.Running : CurrentDeviceState.ReadyToRun;
        }

        private CancellationTokenSource source = new CancellationTokenSource();

        private async void OnInitializeMatchButtonClick(object sender, EventArgs e)
        {
            List<IDriverStationConfiguration> configurations =
                new List<IDriverStationConfiguration>(m_fmsAllianceStations.Count);
            // Grab all DS Data.
            int invalid = -1;
            foreach (var value in m_fmsAllianceStations.Values)
            {
                configurations.Add(value.GetCurrentState(invalid--));
            }

            int matchNum = 0;
            if (!int.TryParse(m_matchNumberView.Text, out matchNum))
            {
                matchNum = 1;
            }

            using (var scope = m_retainedState.AutoFacContainer.BeginLifetimeScope())
            {
                var client = scope.Resolve<DriverStationClient>();
                bool success = await client.UpdateDriverStationConfigurations(configurations, matchNum, 0, source.Token);
                if (success) SetCleanSettings();
            }
        }

        private void OnFmsConnectionChanged(bool connected)
        {
            if (connected) FmsConnected();
            else FmsDisconnected();
        }

        private void FmsConnected()
        {
            using (var scope = m_retainedState.AutoFacContainer.BeginLifetimeScope())
            {
                var dsClient = scope.Resolve<DriverStationClient>();

                var report = dsClient.GetDriverStationReports();

                foreach (var s in report)
                {
                    var station = m_fmsAllianceStations[s.Key];
                    station.UpdateStationReport(s.Value);
                }
                source = new CancellationTokenSource();
                foreach (var fmsAllianceStation in m_fmsAllianceStations)
                {
                    fmsAllianceStation.Value.DriverStationReconnect();
                }
                m_matchTiming.DriverStationReconnect();

                bool foundValidTeamNumber = false;
                foreach (var fmsAllianceStation in m_fmsAllianceStations)
                {
                    if (fmsAllianceStation.Value.TeamNumber >= 0)
                    {
                        foundValidTeamNumber = true;
                        break;
                    }
                }
                if (!foundValidTeamNumber)
                    SetDirtySettings();
            }
        }

        private void FmsDisconnected()
        {
            foreach (var fmsAllianceStation in m_fmsAllianceStations)
            {
                fmsAllianceStation.Value.SetFMSDisconnected();

            }

            source.Cancel();
            foreach (var fmsAllianceStation in m_fmsAllianceStations)
            {
                fmsAllianceStation.Value.DriverStationDisconnect();
            }
            m_matchTiming.DriverStationDisconnect();
            SetDirtySettings();
        }

        private IReadOnlyDictionary<string, int> GetIdConstants()
        {
            Dictionary<string, int> constants = new Dictionary<string, int>();

            FieldInfo[] infos = typeof(Resource.Id).GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var fieldInfo in infos)
            {
                if (!fieldInfo.IsInitOnly && fieldInfo.IsLiteral)
                {
                    object val = fieldInfo.GetValue(null);
                    if (val is int)
                    {
                        constants.Add(fieldInfo.Name, (int)val);
                    }
                }
            }

            return constants;
        }
    }
}