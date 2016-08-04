using System;
using System.Drawing;
using System.Windows.Forms;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.WinForms.CheckBoxes;

namespace SimpleFMS.WinForms.Panels
{
    public class StationPanel : Panel
    {
        public AllianceStationNumber StationNumber { get; }

        private readonly TextBox m_teamNumber;
        private readonly BigCheckBox m_bypass;

        private readonly Panel m_robotConnected;
        private readonly Panel m_dsConnected;

        public const int PanelWidth = 196;
        public const int PanelHeight = 80;

        public StationPanel(AllianceStationNumber number, Color backColor)
        {
            StationNumber = number;

            Size = new Size(PanelWidth, PanelHeight);


            BackColor = backColor;

            m_teamNumber = new TextBox
            {
                Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Size = new Size(70, 40),
                Location = new Point(13, 35)
            };
            Controls.Add(m_teamNumber);

            m_teamNumber.TextChanged += OnTextChanged;

            m_bypass = new BigCheckBox()
            {
                Size = new Size(38, 38),
                Location = new Point(92, 35),
                Checked = true
            };
            Controls.Add(m_bypass);

            var label = new Label
            {
                AutoSize = true,
                Text = "Team Number",
                Font = new Font("Microsoft Sans Serif", 20.25F, FontStyle.Regular, GraphicsUnit.Point, 0),
                Location = new Point(3, 3)
            };
            Controls.Add(label);

            m_dsConnected = new Panel
            {
                BackColor = Color.Red,
                Size = new Size(19, 38),
                Location = new Point(138, 35)
            };
            Controls.Add(m_dsConnected);

            m_robotConnected = new Panel
            {
                BackColor = Color.Red,
                Size = new Size(19, 38),
                Location = new Point(157, 35)
            };
            Controls.Add(m_robotConnected);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box != null)
            {
                int teamNumber = 0;
                if (int.TryParse(box.Text, out teamNumber))
                {
                    m_bypass.Checked = false;
                }
                else
                {
                    m_bypass.Checked = true;
                }
            }
        }

        public void ResetPanel()
        {
            UpdateConnectionStation(false, false);
        }

        public IDriverStationConfiguration GetState(AllianceStationSide side, int teamNumberToSet)
        {
            DriverStationConfiguration ds = new DriverStationConfiguration();
            ds.IsBypassed = m_bypass.Checked;
            ds.Station = new AllianceStation(side, StationNumber);

            int teamNumber = 0;
            if (int.TryParse(m_teamNumber.Text, out teamNumber))
            {
                ds.TeamNumber = teamNumber;
            }
            else
            {
                // Failed to parse. Set team number and bypassed
                ds.IsBypassed = true;
                ds.TeamNumber = teamNumberToSet;
                Invoke((MethodInvoker) delegate
                {
                    m_teamNumber.Text = teamNumberToSet.ToString();
                    m_bypass.Checked = true;
                });
            }
            return ds;
        }

        public void UpdateConnectionStation(bool? dsConnection, bool? roboRioConnection)
        {
            if (dsConnection.HasValue)
            {
                // Updating DS Connection Status
                if (dsConnection.Value)
                {
                    m_dsConnected.BackColor = Color.GreenYellow;
                }
                else
                {
                    // Driver Station Disconnected
                    m_dsConnected.BackColor = Color.Red;
                }
            }

            if (roboRioConnection.HasValue)
            {
                // Updating DS Connection Status
                if (roboRioConnection.Value)
                {
                    m_robotConnected.BackColor = Color.GreenYellow;
                }
                else
                {
                    // Driver Station Disconnected
                    m_robotConnected.BackColor = Color.Red;
                }
            }

        }

        public void Disable()
        {
            m_teamNumber.Enabled = false;
            m_bypass.Enabled = false;
        }

        public void Enable()
        {
            m_teamNumber.Enabled = true;
            m_bypass.Enabled = true;
        }
    }
}
