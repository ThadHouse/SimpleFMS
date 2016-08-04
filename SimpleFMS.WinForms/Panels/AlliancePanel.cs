﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SimpleFMS.Base.DriverStation.Interfaces;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.WinForms.Panels
{
    public class AlliancePanel : Panel
    {
        public AllianceStationSide AllianceSide { get; }

        private readonly List<StationPanel> m_panels = new List<StationPanel>(3);

        public AlliancePanel(AllianceStationSide side)
        {
            AllianceSide = side;

            Size = new Size(196, 270);

            BackColor = GetAllianceColor(side);

            int height = 5;

            bool first = true;
            foreach (AllianceStationNumber number in Enum.GetValues(typeof(AllianceStationNumber)))
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Panel sepPanel = new Panel();
                    sepPanel.Size = new Size(StationPanel.PanelWidth, 10);
                    sepPanel.BackColor = Color.Black;
                    sepPanel.Location = new Point(0, height);
                    height += 10;
                    Controls.Add(sepPanel);
                }

                StationPanel panel = new StationPanel(number, BackColor);
                panel.Location = new Point(0, height);
                m_panels.Add(panel);
                Controls.Add(panel);

                height += StationPanel.PanelHeight;
            }
        }

        public void ResetPanel()
        {
            foreach (var stationPanel in m_panels)
            {
                stationPanel.ResetPanel();
            }
        }

        public void GetDriverStationConfigurations(ref List<IDriverStationConfiguration> configurations, ref int badStationNumber)
        {
            foreach (var stationPanel in m_panels)
            {
                var state = stationPanel.GetState(AllianceSide, badStationNumber);
                configurations.Add(state);
                badStationNumber--;
            }
            ;
        }

        public void UpdateDriverStationConfiguration(AllianceStationNumber number, bool? dsConnected, bool? rioConnected)
        {
            foreach (var stationPanel in m_panels)
            {
                if (stationPanel.StationNumber == number)
                {
                    stationPanel.UpdateConnectionStation(dsConnected, rioConnected);
                }
            }
        }

        private Color GetAllianceColor(AllianceStationSide side)
        {
            if (side == AllianceStationSide.Red)
            {
                return Color.IndianRed;
            }
            else if (side == AllianceStationSide.Blue)
            {
                return Color.CornflowerBlue;
            }
            else
            {
                return Color.LightGray;
            }
        }
    }
}
