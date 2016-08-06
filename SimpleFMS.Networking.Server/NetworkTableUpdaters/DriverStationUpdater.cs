using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.Networking.Base.Extensions.DriverStation;
using static SimpleFMS.Base.Networking.NetworkingConstants.DriverStationConstants;
using static SimpleFMS.Networking.Base.Extensions.DriverStation.DriverStationConfigurationExtensions;
using static SimpleFMS.Networking.Base.Extensions.DriverStation.DriverStationBypassExtensions;
using static SimpleFMS.Networking.Base.Extensions.DriverStation.DriverStationEStopExtensions;

namespace SimpleFMS.Networking.Server.NetworkTableUpdaters
{
    internal sealed class DriverStationUpdater : NetworkTableUpdaterBase
    {
        private readonly IDriverStationManager m_driverStationManager;
        private readonly StandaloneRemoteProcedureCall m_rpc;

        public DriverStationUpdater(ITable root, StandaloneRemoteProcedureCall rpc, IDriverStationManager dsManager)
            : base(root, DriverStationTableName)
        {
            m_rpc = rpc;
            m_driverStationManager = dsManager;
            m_rpc.CreateRpc(DriverStationSetConfigurationRpcKey,
                new RpcDefinition(DriverStationSetConfigurationRpcVersion, DriverStationSetConfigurationRpcKey),
                DriverStationTeamUpdateRpcCallback);

            m_rpc.CreateRpc(DriverStationUpdateBypassRpcKey,
                new RpcDefinition(DriverStationUpdateBypassRpcVersion, DriverStationUpdateBypassRpcKey),
                DriverStationUpdateBypassRpcCallback);

            m_rpc.CreateRpc(DriverStationUpdateEStopRpcKey,
                new RpcDefinition(DriverStationUpdateEStopRpcVersion, DriverStationUpdateEStopRpcKey),
                DriverStationUpdateEStopRpcCallback);

            m_rpc.CreateRpc(DriverStationGlobalEStopRpcKey,
                new RpcDefinition(DriverStationGlobalEStopVersion, DriverStationGlobalEStopRpcKey),
                DriverStationGlobalEStopRpcCallback);
        }

        private byte[] DriverStationUpdateBypassRpcCallback(string name, byte[] bytes)
        {
            bool isValid = false;
            bool bypass = false;
            var stationToBypass = bytes.GetDriverStationToBypass(out bypass, out isValid);

            if (!isValid) return PackDriverStationUpdateBypassResponse(false);
            m_driverStationManager.SetBypass(stationToBypass, bypass);
            return PackDriverStationUpdateBypassResponse(true);
        }

        private byte[] DriverStationUpdateEStopRpcCallback(string name, byte[] bytes)
        {
            bool isValid = false;
            bool eStop = false;
            var stationToEStop = bytes.GetDriverStationToEStop(out eStop, out isValid);

            if (!isValid) return PackDriverStationUpdateEStopResponse(false);
            m_driverStationManager.SetEStop(stationToEStop, eStop);
            return PackDriverStationUpdateEStopResponse(true);
        }

        private byte[] DriverStationTeamUpdateRpcCallback(string name, byte[] bytes)
        {
            // Receiving the raw byte[]
            int matchNumber = 0;
            MatchType matchType = 0;
            var configurations = bytes.GetDriverStationConfigurations(out matchNumber, out matchType);
            bool set = m_driverStationManager.InitializeMatch(configurations, matchNumber, matchType);
            return PackDriverStationSetConfigurationResponse(set);
        }

        private byte[] DriverStationGlobalEStopRpcCallback(string name, byte[] bytes)
        {
            bool valid = bytes.GetGlobalDriverStationEStop();
            if (valid)
            {
                for (byte i = 0; i < 6; i++)
                {
                    m_driverStationManager.SetEStop(new AllianceStation(i), true);
                }
            }
            return PackDriverStationGlobalEStopResponse();
        }

        public override void UpdateTable()
        {
            var driverStations = m_driverStationManager.DriverStations;

            NetworkTable.PutRaw(DriverStationReportKey, driverStations.PackDriverStationReportData());
        }
    }
}
