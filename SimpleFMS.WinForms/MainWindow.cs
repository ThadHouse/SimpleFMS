using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using SimpleFMS.DriverStation;
using SimpleFMS.DriverStation.Base.Enums;
using SimpleFMS.DriverStation.Base.Interfaces;
using SimpleFMS.NetworkTables;
using SimpleFMS.WinForms.Panels;

namespace SimpleFMS.WinForms
{
    public partial class MainWindow : Form, IDriverStationConnectionResponder
    {
        private readonly AlliancesPanel m_alliancePanel;
        private readonly MatchStatePanel m_matchTimePanel;

        private Button m_updateDsButton;

        readonly DriverStationManager m_manger;
        private DriverStationNetworkTablesController m_networkTableController;

        public MainWindow()
        {
            InitializeComponent();

            m_networkTableController = new DriverStationNetworkTablesController();
            m_manger = new DriverStationManager(m_networkTableController, this);
            

            m_alliancePanel = new AlliancesPanel();
            m_alliancePanel.Location = new Point(20, 20);
            this.Controls.Add(m_alliancePanel);

            m_matchTimePanel = new MatchStatePanel(m_manger);
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

        private void OnInitializeMatchButtonClick(object sender, EventArgs e)
        {
            // Grab all DS Data.
            var list = m_alliancePanel.GetDriverStationConfigurations();
            m_manger.InitializeMatch(list, 1, 0);
            m_matchTimePanel.EnableStart();
        }

        public void OnDriverStationConnectionChanged(IDriverStationConfiguration configuration, bool connected)
        {
            Invoke((MethodInvoker)delegate
            {
                m_alliancePanel.UpdateDriverStationConnectionInfo(configuration.StationNumber, configuration.AllianceSide, connected, null);
            });
        }

        public void OnRobotConnectionChanged(IDriverStationConfiguration configuration, bool connected)
        {
            Invoke((MethodInvoker)delegate
            {
                m_alliancePanel.UpdateDriverStationConnectionInfo(configuration.StationNumber, configuration.AllianceSide, null, connected);
            });
        }

        public void AllConnectionReset()
        {
            // No Op right now
        }
    }
}
