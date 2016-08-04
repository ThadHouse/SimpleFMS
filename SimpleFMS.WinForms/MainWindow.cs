﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.MatchTiming;
using SimpleFMS.Base.Networking;
using SimpleFMS.DriverStation;
using SimpleFMS.MatchTiming;
using SimpleFMS.Networking.Server;
using SimpleFMS.WinForms.Panels;

namespace SimpleFMS.WinForms
{
    public partial class MainWindow : Form
    {
        private readonly AlliancesPanel m_alliancePanel;
        private readonly MatchStatePanel m_matchTimePanel;

        private Button m_updateDsButton;

        readonly IDriverStationManager m_driverStationManager;

        private readonly INetworkServerManager m_networkManager;

        private readonly IMatchTimingManager m_matchTimingManager;

        public MainWindow()
        {
            InitializeComponent();

            m_driverStationManager = new DriverStationManager();
            m_matchTimingManager = new MatchTimingManager(m_driverStationManager);

            m_networkManager = new NetworkServerManager(m_driverStationManager, m_matchTimingManager);

            m_driverStationManager.OnDriverStationStatusChanged += MangerOnOnDriverStationStatusChanged;
            

            m_alliancePanel = new AlliancesPanel();
            m_alliancePanel.Location = new Point(20, 20);
            this.Controls.Add(m_alliancePanel);

            m_matchTimePanel = new MatchStatePanel(m_matchTimingManager);
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

        private void OnInitializeMatchButtonClick(object sender, EventArgs e)
        {
            // Grab all DS Data.
            var list = m_alliancePanel.GetDriverStationConfigurations();
            m_driverStationManager.InitializeMatch(list, 1, 0);
            m_matchTimePanel.EnableStart();
        }

        public void OnDriverStationConnectionChanged(IDriverStationConfiguration configuration, bool connected)
        {
            Invoke((MethodInvoker)delegate
            {
                m_alliancePanel.UpdateDriverStationConnectionInfo(configuration.Station.StationNumber, configuration.Station.AllianceSide, connected, null);
            });
        }

        public void OnRobotConnectionChanged(IDriverStationConfiguration configuration, bool connected)
        {
            Invoke((MethodInvoker)delegate
            {
                m_alliancePanel.UpdateDriverStationConnectionInfo(configuration.Station.StationNumber, configuration.Station.AllianceSide, null, connected);
            });
        }

        public void AllConnectionReset()
        {
            // No Op right now
        }
    }
}
