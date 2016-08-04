using SimpleFMS.DriverStation.Enums;
using SimpleFMS.DriverStation.Extensions;

namespace SimpleFMS.DriverStation.TcpData
{
    public class DriverStationConnectionStatusData 
    {
        public int TeamNumber { get; set; }
        public DriverStationConnectionPacketType Status { get; set; }

        // Packet Structure - Tcp Receive From Driver Station
        // Note everything is Big Endian
        // Also, using the size here isn't as reliable, but 3 bytes are
        // always sent at the minimum, so we can just straight check packet type
        // [0-1] Number of bytes sent after the 2 size bytes. Usually 3 for status "24", and 2 for status "28"
        // [2] The packet type. We care about "24" and "28"
        // For Status "24"
        // [3-4] Team Number
        // Status 28 is a keep alive packet, as you need to keep the TCP channel open for the DS to stay connected
        // 
        // When packet type "24" is received, that is when packet type "25" needs to be sent back.

        public ReceiveParseStatus ParseData(byte[] data)
        {
            // Too short to be a good packet
            if (data == null || data.Length < 3)
                return ReceiveParseStatus.InvalidPacket;

            // First 2 bytes are size, and should be 3
            // Big Endian
            if (data[2] == (int)DriverStationConnectionPacketType.SetTeam)
            {
                // Need 5 bytes
                if (data.Length < 5) return ReceiveParseStatus.TrackedPacketInvalid;
                // 3rd byte is status, needs to be 24
                Status = (DriverStationConnectionPacketType)data[2];
                int index = 3;
                TeamNumber = data.GetUShort(ref index);
                return ReceiveParseStatus.TrackedPacketValid;
            }
            // DS Ping
            else if (data[2] == (int)DriverStationConnectionPacketType.Ping)
            {
                Status = (DriverStationConnectionPacketType)data[2];
                return ReceiveParseStatus.TrackedPacketValid;
            }
            return ReceiveParseStatus.UntrackedPacket;
        }
    }
}
