using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleFMS.DriverStation.Base.Enums;

namespace SimpleFMS.DriverStation
{
    public static class GlobalDriverStationSettings
    {
        public const int UdpSendPort = 1120;
        public const int UdpReceivePort = 1160;
        public const int TcpListenPort = 1750;

        public static readonly int MaxNumDriverStations = GetMaxNumDriverStations();

        private static int GetMaxNumDriverStations()
        {
            int count = 0;
            foreach (var side in Enum.GetValues(typeof(AllianceStationSide)))
            {
                foreach (var number in Enum.GetValues(typeof(AllianceStationNumber)))
                {
                    count++;
                }
            }
            return count;
        }
    }
}
