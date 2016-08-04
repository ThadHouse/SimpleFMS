using SimpleFMS.Base.DriverStation;
using SimpleFMS.DriverStation.Extensions;

namespace SimpleFMS.DriverStation.UdpData
{
    public class DriverStationControlData
    {
        public AllianceStation Station { get; set; }
        public bool IsEStopped { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsAutonomous => GlobalDriverStationControlData.IsAutonomous;
        public ushort PacketNumber { get; set; }

        private byte GetControlWord()
        {
            byte controlWord = 0;
            unchecked
            {
                if (IsAutonomous) controlWord |= 0x02;
                if (IsEnabled) controlWord |= 0x04;
                if (IsEStopped) controlWord |= 0x80;
            }
            return controlWord;
        }
        // Packet Strucute - Udp Send To Driver Station
        // Note everything is Big Endian
        // Send 22 bytes
        // [0-1] Packet Number (unsigned 16 bit value)
        // [2] Protocol Version (0 works fine, but give a popup on the DS. I'll try and figure out the actual version)
        // [3] Control word byte. Listed below is the masks for it
        // [4] 0 
        // [5] Alliance Station. See Tcp Send Packet for details
        // [6] Match Type. 1 = Practice, 2 = Quals, 3 = Elims
        // [7-8] Match Number (unsigned 16 bit value)
        // [9] Match repeat number (1 for first match)
        // [10-13] Current milliseconds * 1000
        // [14] Current seconds
        // [15] Current minute
        // [16] Current hour
        // [17] Current day
        // [18] Current month
        // [19] Current Year - 1900
        // [20-21] Remaining match time in seconds (unsinged 16 bit value)
         


        public byte[] PackData()
        {
            byte[] array = new byte[22];

            byte controlWord = GetControlWord();
            int index = 0;
            PacketNumber.AddToArray(array, ref index);
            PacketNumber++;
            if (PacketNumber == 0) PacketNumber = 1;
            array[index] = 0;
            index++;
            array[index] = controlWord;
            index++;
            array[index] = 0;
            index++;
            array[index] = Station.GetByte();
            index++;
            array[index] = (byte)GlobalDriverStationControlData.MatchType;
            index++;
            ((short)GlobalDriverStationControlData.MatchNumber).AddToArray(array, ref index);
            array[index] = 0;
            index++;
            GlobalDriverStationControlData.FmsTime.AddToArray(array, ref index);
            ((ushort)GlobalDriverStationControlData.MatchTimeRemaining).AddToArray(array, ref index);
            return array;
        }
    }
}
