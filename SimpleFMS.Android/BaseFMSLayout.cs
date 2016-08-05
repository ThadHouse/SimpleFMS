using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Autofac;
using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Networking;
using SimpleFMS.Networking.Base;
using SimpleFMS.Networking.Client;
using SimpleFMS.Networking.Client.NetworkClients;
using static SimpleFMS.Android.AutoFacContainer;

namespace SimpleFMS.Android
{
    [Activity(Label = "Simple FMS Android Controller", MainLauncher = true)]
    public class BaseFMSLayout : Activity
    {
        private Dictionary<AllianceStation, FmsAllianceStation> m_fmsAllianceStations =
            new Dictionary<AllianceStation, FmsAllianceStation>(AllianceStationConstants.MaxNumDriverStations);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainFmsScreen);

            // Create your application here
            if (GlobalContainer == null)
            {
                INetworkClientManager networkManager = new NetworkClientManager("Android FMS Client");
                DriverStationClient dsClient = new DriverStationClient(networkManager);
                MatchTimingClient matchClient = new MatchTimingClient(networkManager);

                networkManager.AddClient(dsClient);
                networkManager.AddClient(matchClient);

                networkManager.NetworkTable.AddConnectionListener(OnNetworkTableConnection, true);

                var builder = new ContainerBuilder();
                builder.RegisterInstance(networkManager).As<INetworkClientManager>();
                builder.RegisterInstance(dsClient).As<DriverStationClient>();
                builder.RegisterInstance(matchClient).As<MatchTimingClient>();

                GlobalContainer = builder.Build();
            }

            var IdConstants = GetIdConstants();

            for (int i = 1; i <= AllianceStationConstants.MaxNumDriverStations; i++)
            {
                EditText teamNumber = FindViewById<EditText>(IdConstants[$"Station{i}TeamNumber"]);
                CheckBox bypass = FindViewById<CheckBox>(IdConstants[$"Station{i}Bypass"]);
                View dsComm = FindViewById<View>(IdConstants[$"Station{i}DSComm"]);
                View rioComm = FindViewById<View>(IdConstants[$"Station{i}RioComm"]);
                View eStop = FindViewById<View>(IdConstants[$"Station{i}EStop"]);

                AllianceStation station = new AllianceStation((byte)(i - 1));
                int defaultTeamNumber = (i) * -1;

                FmsAllianceStation fmsStation = new FmsAllianceStation(teamNumber, bypass, dsComm, rioComm, eStop, this,
                    station, defaultTeamNumber);
                m_fmsAllianceStations.Add(station, fmsStation);
            }

            var initializeMatchButton = FindViewById<Button>(Resource.Id.initializeMatchButton);

            initializeMatchButton.Click += OnInitializeMatchButtonClick;
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


            using (var scope = GlobalContainer.BeginLifetimeScope())
            {
                var client = scope.Resolve<DriverStationClient>();
                await client.UpdateDriverStationConfigurations(configurations, 1, 0, source.Token);
            }
        }

        private void OnNetworkTableConnection(IRemote remote, ConnectionInfo info, bool connected)
        {
            if (info.RemoteId == NetworkingConstants.ServerRemoteName)
            {
                RunOnUiThread(() =>
                {
                    if (connected) OnFMSConnected();
                    else OnFMSDisconnected();
                }); ;
            }
        }

        private Activity mainFmsWindow;

        private void OnFMSConnected()
        {
            using (var scope = GlobalContainer.BeginLifetimeScope())
            {
                var dsClient = scope.Resolve<DriverStationClient>();

                var report = dsClient.GetDriverStationReports();

                foreach (var s in report)
                {
                    var station = m_fmsAllianceStations[s.Key];
                    station.TeamNumber = s.Value.TeamNumber;
                    station.UpdateStationConnection(s.Value.DriverStationConnected, s.Value.RoboRioConnected,
                        s.Value.IsBeingSentEStopped || s.Value.IsReceivingEStopped);
                }
            }
        }

        private void OnFMSDisconnected()
        {
            foreach (var fmsAllianceStation in m_fmsAllianceStations)
            {
                fmsAllianceStation.Value.SetFMSDisconnected();
            }
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