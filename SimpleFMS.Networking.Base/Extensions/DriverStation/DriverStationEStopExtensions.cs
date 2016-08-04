﻿using System;
using SimpleFMS.Base.DriverStation;

namespace SimpleFMS.Networking.Base.Extensions.DriverStation
{
    public static class DriverStationEStopExtensions
    {
        public static byte[] PackDriverStationSetEStop(this AllianceStation station, bool eStop)
        {
            byte[] data = new byte[3];
            data[0] = (byte)CustomNetworkTableType.DriverStationUpdateEStop;
            data[1] = station.GetByte();
            data[2] = (byte)(eStop ? 1 : 0);
            return data;
        }

        public static AllianceStation GetDriverStationToEStop(this byte[] value, out bool eStop, out bool isValid)
        {
            if (value.Length < 3)
                throw new IndexOutOfRangeException("Value must have at least a length of 3");

            eStop = false;
            isValid = false;

            if (value[0] != (byte)CustomNetworkTableType.DriverStationUpdateEStop) return new AllianceStation();
            var station = new AllianceStation(value[1]);
            eStop = value[2] != 0;
            isValid = true;
            return station;
        }

        public static byte[] PackDriverStationUpdateEStopResponse(bool set)
        {
            return new[] { (byte)CustomNetworkTableType.DriverStationUpdateEStop, (byte)(set ? 1 : 0) };
        }

        public static bool UnpackDriverStationUpdateEStopResponse(this byte[] value)
        {
            if (value.Length < 2)
                return false;
            if (value[0] != (byte)CustomNetworkTableType.DriverStationUpdateEStop)
                return false;
            return value[1] != 0;
        }
    }
}
