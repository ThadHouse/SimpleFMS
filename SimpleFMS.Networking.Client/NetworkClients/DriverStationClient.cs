using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.Networking.Base.Extensions.DriverStation;
using static SimpleFMS.Base.Networking.NetworkingConstants.DriverStationConstants;

namespace SimpleFMS.Networking.Client.NetworkClients
{
    public class DriverStationClient : NetworkClientBase
    {
        private readonly StandaloneRemoteProcedureCall m_rpc;

        public DriverStationClient(ITable root, StandaloneRemoteProcedureCall rpc) 
            : base(root, DriverStationTableName)
        {
            m_rpc = rpc;

            AddTableListener(DriverStationReportKey, OnDriverStationReportCallback);
        }

        public event Action<IReadOnlyDictionary<AllianceStation, IDriverStationReport>> OnDriverStationReportsChanged;

        public void OnDriverStationReportCallback(ITable table, string key, Value value, NotifyFlags flags)
        {
            if (OnDriverStationReportsChanged == null) return;
            if (!value.IsRaw()) return;
            if (!key.Contains(DriverStationReportKey)) return;
            var data = value.GetRaw().GetDriverStationReports();
            if (data == null) return;
            OnDriverStationReportsChanged?.Invoke(data);
        }

        public IReadOnlyDictionary<AllianceStation, IDriverStationReport> GetDriverStationReports()
        {
            var value = NetworkTable.GetRaw(DriverStationReportKey, null);
            return value?.GetDriverStationReports();
        }

        public override void Dispose()
        {
            OnDriverStationReportsChanged = null;
            base.Dispose();
        }

        public async Task<bool> UpdateDriverStationEStop(AllianceStation station, bool eStopped, CancellationToken token)
        {
            long callId = m_rpc.CallRpc(DriverStationUpdateBypassRpcKey,
                station.PackDriverStationSetEStop(eStopped));

            byte[] data = await m_rpc.GetRpcResultAsync(callId, token);
            if (data == null) return false;
            return data.UnpackDriverStationUpdateEStopResponse();
        }

        public async Task<bool> UpdateDriverStationBypass(AllianceStation station, bool bypass, CancellationToken token)
        {
            long callId = m_rpc.CallRpc(DriverStationUpdateBypassRpcKey,
                station.PackDriverStationSetBypass(bypass));

            byte[] data = await m_rpc.GetRpcResultAsync(callId, token);
            if (data == null) return false;
            return data.UnpackDriverStationUpdateBypassResponse();
        }

        public async Task<bool> UpdateDriverStationConfigurations(
            IReadOnlyList<IDriverStationConfiguration> configuration, int matchNumber, MatchType matchType, 
            CancellationToken token)
        {
            long callId = m_rpc.CallRpc(DriverStationSetConfigurationRpcKey,
                configuration.PackDriverStationConfigurationData(matchNumber, matchType));

            byte[] data = await m_rpc.GetRpcResultAsync(callId, token);
            if (data == null) return false;
            return data.UnpackDriverStationSetConfigurationResponse();
        }
    }
}
