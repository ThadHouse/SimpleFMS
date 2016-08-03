using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SimpleFMS.Base.DriverStation.Enums;
using SimpleFMS.Base.DriverStation.Interfaces;

namespace SimpleFMS.WinForms.Panels
{
    public class AlliancesPanel : Panel
    {
        private readonly List<AlliancePanel> m_alliancePanels = new List<AlliancePanel>(2);

        private List<IDriverStationConfiguration> m_driverStationConfigurations =
            new List<IDriverStationConfiguration>(6);

        public AlliancesPanel()
        {
            AutoSize = true;

            int width = 5;
            bool first = true;
            foreach (AllianceStationSide side in Enum.GetValues(typeof(AllianceStationSide)))
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Panel sepPanel = new Panel();
                    sepPanel.Size = new Size(10, 60);
                    sepPanel.Location = new Point(width, 0);
                    Controls.Add(sepPanel);
                    width += 10;
                }
                AlliancePanel panel = new AlliancePanel(side);
                panel.Location = new Point(width, 0);
                Controls.Add(panel);
                m_alliancePanels.Add(panel);
                width += StationPanel.PanelWidth;
            }
        }

        public void ResetPanel()
        {
            foreach (var alliancePanel in m_alliancePanels)
            {
                alliancePanel.ResetPanel();
            }
        }

        public IReadOnlyList<IDriverStationConfiguration> GetDriverStationConfigurations()
        {
            m_driverStationConfigurations.Clear();
            foreach (var alliancePanel in m_alliancePanels)
            {
                alliancePanel.GetDriverStationConfigurations(ref m_driverStationConfigurations);
            }
            return m_driverStationConfigurations;
        }

        public void UpdateDriverStationConnectionInfo(AllianceStationNumber number, AllianceStationSide side, bool? dsConnection, bool? roboRioConnection)
        {
            foreach (var alliancePanel in m_alliancePanels)
            {
                if (alliancePanel.AllianceSide == side)
                {
                    alliancePanel.UpdateDriverStationConfiguration(number, dsConnection, roboRioConnection);
                }
            }
        }

    }
}
