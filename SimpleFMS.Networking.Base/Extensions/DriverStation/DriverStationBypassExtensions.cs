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
            bypass = false;
            isValid = false;

            if (value.Length < 3)
                return new AllianceStation();

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
