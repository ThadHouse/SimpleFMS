﻿using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.Base.Networking;
using SimpleFMS.Networking.Base.Extensions;
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

        public DriverStationUpdater(ITable root, IDriverStationManager dsManager)
            : base(root, DriverStationTableName)
        {
            m_driverStationManager = dsManager;
            RemoteProcedureCall.CreateRpc(DriverStationSetConfigurationRpcKey,
                new RpcDefinition(DriverStationSetConfigurationRpcVersion, DriverStationSetConfigurationRpcKey),
                DriverStationTeamUpdateRpcCallback);

            RemoteProcedureCall.CreateRpc(DriverStationUpdateBypassRpcKey,
                new RpcDefinition(DriverStationUpdateBypassRpcVersion, DriverStationUpdateBypassRpcKey),
                DriverStationUpdateBypassRpcCallback);

            RemoteProcedureCall.CreateRpc(DriverStationUpdateEStopRpcKey,
                new RpcDefinition(DriverStationUpdateEStopRpcVersion, DriverStationUpdateEStopRpcKey),
                DriverStationUpdateEStopRpcCallback);
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

        public override void UpdateTable()
        {
            var driverStations = m_driverStationManager.DriverStations;

            NetworkTable.PutRaw(DriverStationReportKey, driverStations.PackDriverStationReportData());
        }
    }
}
