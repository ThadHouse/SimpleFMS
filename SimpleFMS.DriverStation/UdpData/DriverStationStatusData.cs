using SimpleFMS.DriverStation.Enums;
using SimpleFMS.DriverStation.Extensions;

namespace SimpleFMS.DriverStation.UdpData
{
    /// <summary>
    /// The status returned from the DriverStations
    /// </summary>
    public class DriverStationStatusData
    {

        public bool HasRobotComms { get; internal set; }

        public bool IsEStopped { get; internal set; }

        public ushort PacketNumber { get; internal set; }

        public bool IsEnabled { get; internal set; }

        public bool IsAutonomous { get; internal set; }

        public int TeamNumber { get; internal set; }

        public double RobotBattery { get; internal set; }

        // Packet Structure - Udp Receive from Driver Station
        // Note everything is Big Endian
        // Minimum packet length for a valid packet is 8 bytes
        // [0-1] Packet Number (unsigned 16 bit value)
        // [2] Protocol Version
        // [3] The robot status byte. Listed below is the masks for it
        // [4-5] Team Number
        // [6-7] Battery Voltage (Divide by 256 to get actual voltage) 

        public ReceiveParseStatus ParseData(byte[] packet)
        {
            if (packet == null || packet.Length < 8) return ReceiveParseStatus.InvalidPacket;

            int index = 0;
            PacketNumber = packet.GetUShort(ref index);
            // Unknown what packet[2] is
            byte status = packet[3];
            IsAutonomous = (status & 0x02) == 0x02;
            IsEnabled = (status & 0x04) == 0x04;
            IsEStopped = (status & 0x80) == 0x80;
            HasRobotComms = (status & 0x20) == 0x20;
            index = 4;
            TeamNumber = packet.GetUShort(ref index);
            RobotBattery = packet.GetUShort(ref index) / 256.0;

            return ReceiveParseStatus.TrackedPacketValid;
        }
    }
}
