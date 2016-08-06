using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Autofac;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Networking.Client.NetworkClients;
using static SimpleFMS.Android.AutoFacContainer;

namespace SimpleFMS.Android
{
    public class FmsAllianceStation
    {
        private EditText m_teamNumberView;
        private CheckBox m_bypassView;
        private View m_dsCommView;
        private View m_rioCommView;
        private View m_eStoppedView;
        private Activity m_parentActivity;

        public AllianceStation Station { get; }

        private int m_teamNumber;
        public int TeamNumber
        {
            get  { return m_teamNumber; }
            set
            {
                int old = m_teamNumber;
                m_teamNumber = value;
                if (value != old)
                {
                    m_parentActivity.RunOnUiThread(() => m_teamNumberView.Text = m_teamNumber.ToString());
                }
            }
        }

        private bool m_bypassed = true;
        public bool Bypass
        {
            get { return m_bypassed; }
            set
            {
                bool old = m_bypassed;
                m_bypassed = value;
                if (value != old)
                {
                    m_parentActivity.RunOnUiThread(() => m_bypassView.Checked = m_bypassed);
                }
            }
        }

        private int m_defaultTeamNumber = 0;

        public FmsAllianceStation(EditText teamNumberView, CheckBox bypassView, View dsCommView, View rioCommView,
            View eStoppedView, Activity parent, AllianceStation station, int startingTeamNumber)
        {
            m_defaultTeamNumber = startingTeamNumber;
            m_teamNumberView = teamNumberView;
            m_bypassView = bypassView;
            m_dsCommView = dsCommView;
            m_rioCommView = rioCommView;
            m_eStoppedView = eStoppedView;
            m_parentActivity = parent;
            Station = station;
            TeamNumber = startingTeamNumber;

            m_teamNumberView.TextChanged += (sender, args) =>
            {
                string newValue = args.Text.ToString();
                int teamNumber = 0;
                if (int.TryParse(newValue, out teamNumber))
                {
                    if (teamNumber < 0)
                    {
                        m_bypassView.Checked = true;
                        teamNumber = m_defaultTeamNumber;
                    }
                    else
                    {
                        m_bypassView.Checked = false;
                    }
                }
                else
                {
                    m_bypassView.Checked = true;
                    teamNumber = m_defaultTeamNumber;
                }
                m_teamNumber = teamNumber;
            };

            m_bypassView.CheckedChange += async (sender, args) =>
            {
                bool newValue = args.IsChecked;
                m_bypassed = newValue;
                bool success = await UpdateStationBypass(newValue);
                // TODO: Handle an unsuccessful set
            };
        }
        private CancellationTokenSource source = new CancellationTokenSource();

        private async Task<bool> UpdateStationBypass(bool newValue)
        {

            using (var scope = GlobalContainer.BeginLifetimeScope())
            {
                var client = scope.Resolve<DriverStationClient>();
                bool success = await client.UpdateDriverStationBypass(Station, newValue, source.Token);
                return success;
            }
        }

        public IDriverStationConfiguration GetCurrentState(int teamNumberToSetIfInvalid)
        {
            DriverStationConfiguration ds = new DriverStationConfiguration();
            ds.IsBypassed = m_bypassView.Checked;
            ds.Station = Station;

            int teamNumber = 0;
            if (!int.TryParse(m_teamNumberView.Text, out teamNumber))
            {
                ds.IsBypassed = true;
                ds.TeamNumber = teamNumberToSetIfInvalid;
                m_parentActivity.RunOnUiThread(() =>
                {
                    m_teamNumberView.Text = teamNumberToSetIfInvalid.ToString();
                    m_teamNumber = teamNumberToSetIfInvalid;
                    m_bypassView.Checked = true;
                });
            }
            else
            {
                ds.TeamNumber = teamNumber;
            }
            return ds;
        }

        public void UpdateStationConnection(bool dsConnection, bool roboRioConnection, bool eStopped)
        {
            m_parentActivity.RunOnUiThread(() =>
            {
                m_dsCommView.SetBackgroundColor(dsConnection ? Color.GreenYellow : Color.Red);

                m_rioCommView.SetBackgroundColor(roboRioConnection ? Color.GreenYellow : Color.Red);

                m_eStoppedView.SetBackgroundColor(eStopped ? Color.Red : Color.GreenYellow);
            });
        }

        public void SetFMSDisconnected()
        {
            m_parentActivity.RunOnUiThread(() =>
            {
                m_eStoppedView.SetBackgroundColor(Color.CornflowerBlue);
            });
        }
    }
}