using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables.Tables;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.MatchTiming;
using SimpleFMS.Base.Networking;
using SimpleFMS.Networking.Base.Extensions.MatchTiming;
using static SimpleFMS.Base.Networking.NetworkingConstants.MatchTimingConstatns;

namespace SimpleFMS.Networking.Server.NetworkTableUpdaters
{
    internal class MatchTimingUpdater : NetworkTableUpdaterBase
    {
        private readonly IMatchTimingManager m_matchTimingManager;

        public MatchTimingUpdater(ITable root, IMatchTimingManager timingManager) 
            : base(root, MatchTimingTableName)
        {
            m_matchTimingManager = timingManager;
        }

        private byte[] StartMatchCallback(string name, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        private byte[] StopPeriodCallback(string name, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        private byte[] StartAutonomousCallback(string name, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        private byte[] StartTeleoperatedCallback(string name, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        private byte[] SetMatchPeriodTimeCallback(string name, byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public override void UpdateTable()
        {
            var report = m_matchTimingManager.GetMatchTimingReport();

            NetworkTable.PutRaw(MatchStatusReportKey, report.PackMatchTimingReport());
        }
    }
}
