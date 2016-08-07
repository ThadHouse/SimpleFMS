using System;
using System.Threading;
using System.Threading.Tasks;
using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.MatchTiming;
using SimpleFMS.Networking.Base;
using SimpleFMS.Networking.Base.Extensions.MatchTiming;
using static SimpleFMS.Base.Networking.NetworkingConstants.MatchTimingConstants;
using static SimpleFMS.Networking.Base.Extensions.MatchTiming.MatchTimingMatchStateExtensions;

namespace SimpleFMS.Networking.Client.NetworkClients
{
    public class MatchTimingClient : NetworkClientBase
    {
        private readonly StandaloneRemoteProcedureCall m_rpc;

        public MatchTimingClient(INetworkClientManager manager) 
            : base(manager.NetworkTable, MatchTimingTableName)
        {
            m_rpc = manager.Rpc;

            AddTableListener(MatchStatusReportKey, OnMatchTimeReportCallback);
        }

        public event Action<IMatchStateReport> OnMatchTimeReportChanged;

        private void OnMatchTimeReportCallback(ITable table, string key, Value value, NotifyFlags flags)
        {
            if (OnMatchTimeReportChanged == null) return;
            if (!value.IsRaw()) return;
            if (!key.Contains(MatchStatusReportKey)) return;
            var data = value.GetRaw().GetMatchTimingReport();
            if (data == null) return;
            OnMatchTimeReportChanged?.Invoke(data);
        }

        public IMatchStateReport GetMatchStateReport()
        {
            var value = NetworkTable.GetRaw(MatchStatusReportKey, null);
            return value?.GetMatchTimingReport();
        }

        public async Task<bool> StartMatch(CancellationToken token)
        {
            long callId = m_rpc.CallRpc(StartMatchRpcKey, PackStartMatch());

            byte[] data = await m_rpc.GetRpcResultAsync(callId, token);
            if (data == null) return false;
            return data.UnpackStartMatchResponse();
        }

        public async Task<bool> StopPeriod(CancellationToken token)
        {
            long callId = m_rpc.CallRpc(StopPeriodRpcKey, PackStopPeriod());

            byte[] data = await m_rpc.GetRpcResultAsync(callId, token);
            if (data == null) return false;
            return data.UnpackStopPeriodResponse();
        }

        public async Task<bool> StartAutonomous(CancellationToken token)
        {
            long callId = m_rpc.CallRpc(StartAutonomousRpcKey, PackStartAutonomous());

            byte[] data = await m_rpc.GetRpcResultAsync(callId, token);
            if (data == null) return false;
            return data.UnpackStartAutonomousResponse();
        }

        public async Task<bool> StartTeleoperated(CancellationToken token)
        {
            long callId = m_rpc.CallRpc(StartTeleoperatedRpcKey, PackStartTeleoperated());

            byte[] data = await m_rpc.GetRpcResultAsync(callId, token);
            if (data == null) return false;
            return data.UnpackStartTeleoperatedResponse();
        }

        public async Task<bool> SetMatchPeriodTimes(IMatchTimeReport times, CancellationToken token)
        {
            long callId = m_rpc.CallRpc(SetMatchPeriodTimeRpcKey, times.PackMatchTimes());

            byte[] data = await m_rpc.GetRpcResultAsync(callId, token);
            if (data == null) return false;
            return data.UnpackMatchTimesResponse();
        }
    }
}
