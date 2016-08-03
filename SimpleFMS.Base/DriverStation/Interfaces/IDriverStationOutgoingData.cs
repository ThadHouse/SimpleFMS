using System.Net;
using SimpleFMS.Base.DriverStation.Enums;

namespace SimpleFMS.Base.DriverStation.Interfaces
{
    public interface IDriverStationOutgoingData
    {
        IPAddress IpAddress { get; }
        AllianceStationSide AllianceSide { get; }
        AllianceStationNumber StationNumber { get; }
        bool IsEStopped { get; }
        bool IsEnabled { get; }
        bool IsAutonomous { get; }
    }
}
