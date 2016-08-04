using System;
using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.MatchTiming;
using SimpleFMS.Networking.Base.Extensions.MatchTiming;
using static SimpleFMS.Base.Networking.NetworkingConstants.MatchTimingConstatns;

namespace SimpleFMS.Networking.Server.NetworkTableUpdaters
{
    internal class MatchTimingUpdater : NetworkTableUpdaterBase
    {
        private readonly IMatchTimingManager m_matchTimingManager;
        private readonly StandaloneRemoteProcedureCall m_rpc;

        public MatchTimingUpdater(ITable root, StandaloneRemoteProcedureCall rpc, IMatchTimingManager timingManager) 
            : base(root, MatchTimingTableName)
        {
            m_rpc = rpc;
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
