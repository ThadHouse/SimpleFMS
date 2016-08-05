using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Autofac;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Networking.Base;
using SimpleFMS.Networking.Client;
using SimpleFMS.Networking.Client.NetworkClients;
using SimpleFMS.WinForms.Panels;

namespace SimpleFMS.WinForms
{
    public partial class MainWindow : Form
    {
        private readonly AlliancesPanel m_alliancePanel;
        private readonly MatchStatePanel m_matchTimePanel;

        private Button m_updateDsButton;

        public static IContainer AutoFacContainer { get; private set; }

        CancellationTokenSource tokenSource = new CancellationTokenSource();

        public MainWindow()
        {
            InitializeComponent();

            var builder = new ContainerBuilder();
            NetworkClientManager nManager = new NetworkClientManager("Win Forms Client");
            DriverStationClient dsClient = new DriverStationClient(nManager.NetworkTable, nManager.Rpc);
            MatchTimingClient matchClient = new MatchTimingClient(nManager.NetworkTable, nManager.Rpc);
            nManager.AddClient(dsClient);
            builder.RegisterInstance(nManager).As<INetworkClientManager>().SingleInstance();
            builder.RegisterInstance(dsClient).As<DriverStationClient>().SingleInstance();
            builder.RegisterInstance(matchClient).As<MatchTimingClient>().SingleInstance();

            AutoFacContainer = builder.Build();

            dsClient.OnDriverStationReportsChanged += MangerOnOnDriverStationStatusChanged;


            m_alliancePanel = new AlliancesPanel();
            m_alliancePanel.Location = new Point(20, 20);
            this.Controls.Add(m_alliancePanel);

            m_matchTimePanel = new MatchStatePanel();
            m_matchTimePanel.Location = new Point(500, 35);
            Controls.Add(m_matchTimePanel);

            m_updateDsButton = new Button
            {
                Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Text = "Initialize New Match",
                Size = new Size(m_alliancePanel.Width, 60),
                Location = new Point(m_alliancePanel.Location.X, m_alliancePanel.Location.Y + m_alliancePanel.Size.Height + 10)
            };
            m_updateDsButton.Click += OnInitializeMatchButtonClick;

            Controls.Add(m_updateDsButton);
        }

        private void MangerOnOnDriverStationStatusChanged(IReadOnlyDictionary<AllianceStation, IDriverStationReport> readOnlyDictionary)
        {
            Invoke((MethodInvoker)delegate
            {
                foreach (var keyValuePair in readOnlyDictionary)
                {
                    m_alliancePanel.UpdateDriverStationConnectionInfo(keyValuePair.Key.StationNumber, keyValuePair.Key.AllianceSide, keyValuePair.Value.DriverStationConnected, keyValuePair.Value.RoboRioConnected);
                }
            });
        }

        private async void OnInitializeMatchButtonClick(object sender, EventArgs e)
        {
            // Grab all DS Data.
            var list = m_alliancePanel.GetDriverStationConfigurations();
            using (var scope = AutoFacContainer.BeginLifetimeScope())
            {
                var client = scope.Resolve <DriverStationClient>();
                await client.UpdateDriverStationConfigurations(list, 1, 0, tokenSource.Token);
            }
            m_matchTimePanel.EnableStart();
        }

        public void AllConnectionReset()
        {
            // No Op right now
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            AutoFacContainer.Dispose();
            
            tokenSource.Cancel();
        }
    }
}
