using NetworkTables.Tables;
using SimpleFMS.Base.DriverStation.Interfaces;
using SimpleFMS.Base.Networking;
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
        }

        public override void UpdateTable()
        {
            var driverStations = m_driverStationManager.DriverStations;

            NetworkTable.PutRaw(NetworkingConstants.DriverStationConstants.DriverStationReportKey,
                driverStations.PackDriverStationReportData());
        }
    }
}
