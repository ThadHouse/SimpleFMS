using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Autofac;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.Networking.Client.NetworkClients;
using SimpleFMS.WinForms.CheckBoxes;

namespace SimpleFMS.WinForms.Panels
{
    public class StationPanel : Panel
    {

        public AllianceStation Station { get; }

        private readonly TextBox m_teamNumber;
        private readonly BigCheckBox m_bypass;

        private readonly Panel m_robotConnected;
        private readonly Panel m_dsConnected;

        public const int PanelWidth = 196;
        public const int PanelHeight = 80;

        public StationPanel(AllianceStation station, Color backColor)
        {
            Station = station;

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

            m_bypass.CheckedChanged += BypassCheckedChanged;

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

        CancellationTokenSource source = new CancellationTokenSource();

        private int m_isUpdatingBypass = 0;

        private async void BypassCheckedChanged(object sender, EventArgs e)
        {
            int result = Interlocked.Exchange(ref m_isUpdatingBypass, 1);
            if (result == 0) return;

            using (var scope = MainWindow.AutoFacContainer.BeginLifetimeScope())
            {
                var ds = scope.Resolve<DriverStationClient>();
                var success = await ds.UpdateDriverStationBypass(Station, m_bypass.Checked, source.Token);
                if (!success) m_bypass.Checked = true;
            }
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
            DriverStationConfiguration ds;

            int teamNumber = 0;
            if (int.TryParse(m_teamNumber.Text, out teamNumber))
            {
                ds = new DriverStationConfiguration(teamNumber, Station, m_bypass.Checked);
            }
            else
            {
                // Failed to parse. Set team number and bypassed
                ds = new DriverStationConfiguration(teamNumberToSet, Station, true);
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
    }
}
