using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleFMS.Base.DriverStation;

namespace SimpleFMS.Networking.Base.Extensions.DriverStation
{
    public static class DriverStationBypassExtensions
    {
        public static byte[] PackDriverStationSetBypass(this AllianceStation station, bool bypass)
        {
            byte[] data = new byte[3];
            data[0] = (byte) CustomNetworkTableType.DriverStationUpdateBypass;
            data[1] = station.GetByte();
            data[2] = (byte) (bypass ? 1 : 0);
            return data;
        }

        public static AllianceStation GetDriverStationToBypass(this byte[] value, out bool bypass, out bool isValid)
        {
            if (value.Length < 3)
                throw new IndexOutOfRangeException("Value must have at least a length of 3");

            bypass = false;
            isValid = false;

            if (value[0] != (byte) CustomNetworkTableType.DriverStationUpdateBypass) return new AllianceStation();
            var station = new AllianceStation(value[1]);
            bypass = value[2] != 0;
            isValid = true;
            return station;
        }

        public static byte[] PackDriverStationUpdateBypassResponse(bool set)
        {
            return new[] { (byte)CustomNetworkTableType.DriverStationUpdateBypass, (byte)(set ? 1 : 0) };
        }

        public static bool UnpackDriverStationUpdateBypassResponse(this byte[] value)
        {
            if (value.Length < 2)
                return false;
            if (value[0] != (byte) CustomNetworkTableType.DriverStationUpdateBypass)
                return false;
            return value[1] != 0;
        }
    }
}
