using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleFMS.Base.DriverStation.Interfaces;
using SimpleFMS.Base.Tuples;

namespace SimpleFMS.Networking.Base
{
    public interface IRequestedDriverStationGatherer
    {
        void GatherRequestedDriverStationData(
            IReadOnlyList<ValueTuple<IDriverStationIncomingData, IDriverStationOutgoingData>>
                driverStationData);
    }
}
