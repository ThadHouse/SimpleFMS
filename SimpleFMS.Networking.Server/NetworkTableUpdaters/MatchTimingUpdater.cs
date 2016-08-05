using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.MatchTiming;
using SimpleFMS.Networking.Base.Extensions.MatchTiming;
using static SimpleFMS.Base.Networking.NetworkingConstants.MatchTimingConstants;
using static SimpleFMS.Networking.Base.Extensions.MatchTiming.MatchTimingMatchStateExtensions;

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

            m_rpc.CreateRpc(StartMatchRpcKey, new RpcDefinition(StartMatchRpcVersion, StartMatchRpcKey),
                StartMatchCallback);

            m_rpc.CreateRpc(StopPeriodRpcKey, new RpcDefinition(StopPeriodRpcVersion, StopPeriodRpcKey),
                StopPeriodCallback);

            m_rpc.CreateRpc(StartAutonomousRpcKey, new RpcDefinition(StartAutonomousRpcVersion, StartAutonomousRpcKey),
                StartAutonomousCallback);

            m_rpc.CreateRpc(StartTeleoperatedRpcKey,
                new RpcDefinition(StartTeleoperatedRpcVersion, StartTeleoperatedRpcKey), StartTeleoperatedCallback);

            m_rpc.CreateRpc(SetMatchPeriodTimeRpcKey,
                new RpcDefinition(SetMatchPeriodTimeRpcVersion, SetMatchPeriodTimeRpcKey), SetMatchPeriodTimeCallback);
        }

        private byte[] StartMatchCallback(string name, byte[] bytes)
        {
            var good = bytes.UnpackStartMatch();
            if (good) m_matchTimingManager.StartMatch();
            return PackStartMatchResponse(good);
        }

        private byte[] StopPeriodCallback(string name, byte[] bytes)
        {
            var good = bytes.UnpackStopPeriod();
            if (good) m_matchTimingManager.StopCurrentPeriod();
            return PackStopPeriodResponse(good);
        }

        private byte[] StartAutonomousCallback(string name, byte[] bytes)
        {
            var good = bytes.UnpackStartAutonomous();
            if (good) m_matchTimingManager.StartAutonomous();
            return PackStartAutonomousResponse(good);
        }

        private byte[] StartTeleoperatedCallback(string name, byte[] bytes)
        {
            var good = bytes.UnpackStartTeleoperated();
            if (good) m_matchTimingManager.StartTeleop();
            return PackStartTeleoperatedResponse(good);
        }

        private byte[] SetMatchPeriodTimeCallback(string name, byte[] bytes)
        {
            var data = bytes.UnpackMatchTimes();
            bool success = false;
            if (data != null)
            {
                success = m_matchTimingManager.SetMatchTimes(data);
            }
            return PackMatchTimesResponse(success);
        }

        public override void UpdateTable()
        {
            var report = m_matchTimingManager.GetMatchTimingReport();

            NetworkTable.PutRaw(MatchStatusReportKey, report.PackMatchTimingReport());
        }
    }
}
