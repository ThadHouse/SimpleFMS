using System.Collections.Generic;
using SimpleFMS.Base.DriverStation.Enums;
using SimpleFMS.Base.Tuples;

namespace SimpleFMS.Base.DriverStation.Interfaces
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
            IReadOnlyList<ValueTuple<IDriverStationIncomingData, IDriverStationOutgoingData>>
                driverStationData);

        void UpdateRequestedDriverStations(IReadOnlyDictionary<ValueTuple<AllianceStationSide, AllianceStationNumber>, int>
            requestedDriverStations);
    }
}
