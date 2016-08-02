using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleFMS.DriverStation.Base.Enums;
using SimpleFMS.DriverStation.Base.Tuples;

namespace SimpleFMS.DriverStation.Base.Interfaces
{
    /// <summary>
    /// Sends a list of all DriverStation data to send over the 
    /// Network to other devices
    /// </summary>
    public interface IDriverStationNetworkSender
    {
        /// <summary>
        /// Sends a list of all Driver Station data to update
        /// </summary>
        /// <param name="driverStationData"></param>
        void UpdateDriverStationNeworkData(
            IReadOnlyList<ImmutableStructTuple<IDriverStationIncomingData, IDriverStationOutgoingData>>
                driverStationData);

        void UpdateRequestedDriverStations(IReadOnlyDictionary<ImmutableStructTuple<AllianceStationSide, AllianceStationNumber>, int>
            requestedDriverStations);
    }
}
