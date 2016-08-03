using System.Collections.Generic;
using SimpleFMS.Base.DriverStation.Interfaces;
using SimpleFMS.Base.Tuples;

namespace SimpleFMS.Base.Networking
{
    public interface IRequestedDriverStationGatherer
    {
        void GatherRequestedDriverStationData(
            IReadOnlyList<ValueTuple<IDriverStationIncomingData, IDriverStationOutgoingData>>
                driverStationData);
    }
}
