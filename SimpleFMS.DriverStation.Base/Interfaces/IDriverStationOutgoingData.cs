using System.Net;
using SimpleFMS.DriverStation.Base.Enums;

namespace SimpleFMS.DriverStation.Base.Interfaces
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
