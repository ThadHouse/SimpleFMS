using SimpleFMS.Base.Enums;
using SimpleFMS.DriverStation.Enums;

namespace SimpleFMS.DriverStation.TcpData
{
    /// <summary>
    /// Control data sent to the DriverStation connection service.
    /// </summary>
    /// <remarks>
    /// This is the data sent over TCP
    /// </remarks>
    public class DriverStationConnectionControlData
    {
        public AllianceStationSide AllianceSide { get; set; }
        public AllianceStationNumber StationNumber { get; set; }
        public AllianceStationStatus StationStatus { get; set; }

        // Packet Structure
        // Note everything is Big Endian
        // [0-1] Number of bytes sent after the 2 size bytes
        // [2] The packet type. We only need to send "24"
        // [3] The alliance station. Values listed below
        // [4] The location status byte
        // 
        // Location Status Layout
        // 0 = Team in correct spot, 1 = Team in wrong spot, 2 = Team not in match
        //
        // Alliance Station Layout
        // 0 = Red1, 1 = Red2, 2 = Red3
        // 3 = Blue1, 4= Blue2, 5 = Blue3

        public byte[] PackData()
        {
            // 5 bytes for now
            byte[] toSend = new byte[5];
            // First 2 bytes are size
            toSend[0] = 3;
            toSend[1] = 0;
            // 3rd byte is a status byte
            toSend[2] = (int)DriverStationConnectionPacketType.SendTeamResponse;

            // 4th byte represents the alliance station
            /*
             * 0 = Red1, 1 = Red2, 2 = Red3
             * 3 = Blue1, 4= Blue2, 5 = Blue3 
             * */
            switch (AllianceSide)
            {
                case AllianceStationSide.Red:
                    toSend[3] = (byte)(StationNumber);
                    break;
                case AllianceStationSide.Blue:
                    toSend[3] = (byte)(StationNumber + 3);
                    break;
            }
            // 5th byte is location status
            toSend[4] = (byte)StationStatus;

            return toSend;
        }
    }
}
