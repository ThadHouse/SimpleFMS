using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkTables;
using NetworkTables.Tables;
using SimpleFMS.Base.DriverStation.Enums;
using SimpleFMS.Base.DriverStation.Interfaces;
using SimpleFMS.Base.Tuples;

namespace SimpleFMS.NetworkTables
{
    public class DriverStationNetworkTablesController : IDriverStationNetworkSender
    {
        public const int NetworkTablePort = 1755;
        public const string NetworkTablePersistentFileName = "FMSPersistentData";

        private IReadOnlyList<ValueTuple<IDriverStationIncomingData, IDriverStationOutgoingData>>
            m_connectedDriverStationData = null;

        private IReadOnlyDictionary<ValueTuple<AllianceStationSide, AllianceStationNumber>, int>
            m_requestedDriverStations = null;

        private readonly ITable m_networkTable;

        private readonly Timer m_updateTimer;

        private readonly object m_lockObject = new object();

        public DriverStationNetworkTablesController()
        {
            NetworkTable.SetPort(NetworkTablePort);
            NetworkTable.SetServerMode();
            NetworkTable.SetNetworkIdentity("SimpleFMS Server");
            NetworkTable.SetPersistentFilename(NetworkTablePersistentFileName);
            NetworkTable.Initialize();
            m_networkTable = NetworkTable.GetTable("FMS");

            m_updateTimer = new Timer(NetworkTableUpdateTimer);
            m_updateTimer.Change(500, 500);
        }

        private void NetworkTableUpdateTimer(object state)
        {
            IReadOnlyList<ValueTuple<IDriverStationIncomingData, IDriverStationOutgoingData>> connected;
            IReadOnlyDictionary<ValueTuple<AllianceStationSide, AllianceStationNumber>, int> requested;
            lock (m_lockObject)
            {
                connected = m_connectedDriverStationData;
                requested = m_requestedDriverStations;
            }

            if (connected != null)
            {
                ConnectedDriverStationStructure connStructs = new ConnectedDriverStationStructure();
                connStructs.DriverStationData = connected;
                var packed = connStructs.PackData();
                if (packed != null)
                {
                    m_networkTable.PutRaw("ConnectedDriverStations", packed);
                }

                lock (m_lockObject)
                {
                    m_connectedDriverStationData = null;
                }
            }

            if (requested != null)
            {
                RequestedDriverStationStructure requestedStructs = new RequestedDriverStationStructure();
                requestedStructs.RequestedDriverStations = requested;
                var packed = requestedStructs.PackData();
                if (packed != null)
                {
                    m_networkTable.PutRaw("RequestedDriverStations", packed);
                }

                lock (m_lockObject)
                {
                    m_requestedDriverStations = null;
                }
            }
        }

        public void UpdateDriverStationNeworkData(IReadOnlyList<ValueTuple<IDriverStationIncomingData, IDriverStationOutgoingData>> driverStationData)
        {
            lock (m_lockObject)
            {
                m_connectedDriverStationData = driverStationData;
            }
        }

        public void UpdateRequestedDriverStations(IReadOnlyDictionary<ValueTuple<AllianceStationSide, AllianceStationNumber>, int>
            requestedDriverStations)
        {
            lock (m_lockObject)
            {
                m_requestedDriverStations = requestedDriverStations;
            }
        }
    }
}
