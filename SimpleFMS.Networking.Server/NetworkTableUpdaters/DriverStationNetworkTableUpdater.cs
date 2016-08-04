using System;
using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.Base.Networking;
using SimpleFMS.Networking.Base;
using SimpleFMS.Networking.Base.Extensions;

namespace SimpleFMS.Networking.Server.NetworkTableUpdaters
{
    internal sealed class DriverStationNetworkTableUpdater : NetworkTableUpdaterBase
    {
        private readonly IDriverStationManager m_driverStationManager;

        public DriverStationNetworkTableUpdater(ITable root, IDriverStationManager dsManager)
            : base(root, NetworkingConstants.DriverStationConstants.DriverStationTableName)
        {
            m_driverStationManager = dsManager;
            RemoteProcedureCall.CreateRpc(NetworkingConstants.DriverStationConstants.DriverStationTeamUpdateRpcKey,
                new RpcDefinition(1, NetworkingConstants.DriverStationConstants.DriverStationTeamUpdateRpcKey),
                DriverStationTeamUpdateRpcCallback);
        }

        private byte[] DriverStationTeamUpdateRpcCallback(string name, byte[] bytes)
        {
            // Receiving the raw byte[]
            int matchNumber = 0;
            MatchType matchType = 0;
            var configurations = bytes.GetDriverStationConfigurations(out matchNumber, out matchType);
            bool set = m_driverStationManager.InitializeMatch(configurations, matchNumber, matchType);
            byte[] retVal = new byte[2];
            retVal[0] = (byte)CustomNetworkTableType.DriverStationConfigurationResponse;
            retVal[1] = (byte)(set ? 1 : 0);
            return retVal;
        }

        public override void UpdateTable()
        {
            var driverStations = m_driverStationManager.DriverStations;

            NetworkTable.PutRaw(NetworkingConstants.DriverStationConstants.DriverStationReportKey,
                driverStations.PackDriverStationReportData());
        }
    }
}
