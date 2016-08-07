using System;
using System.Collections.Generic;
using System.IO;
using NetworkTables.Wire;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Networking;

namespace SimpleFMS.Networking.Base.Extensions.DriverStation
{
    public static class DriverStationReportExtensions
    {
        private const int DriverStationReportSize = 13;

        // Packet Structure
        // Written using NetworkTables WireEncoder
        // [0] Raw Data Type
        // [1] Count
        // Each report in order
        public static byte[] PackDriverStationReportData(this IReadOnlyDictionary<AllianceStation,IDriverStationReport> reports)
        {
            if (reports.Count > 255) 
                throw new ArgumentOutOfRangeException(nameof(reports), "Reports cannot be longer then 255 values");

            WireEncoder encoder = new WireEncoder(NetworkingConstants.NetworkTablesVersion);

            encoder.Write8((byte) CustomNetworkTableType.DriverStationReport);
            encoder.Write8((byte)reports.Count);

            foreach (var report in reports)
            {
                report.PackDriverStationReportData(ref encoder);
            }

            return encoder.Buffer;
        }

        // Packet Structure
        // Written using NetworkTable WireEncoder
        // [0] Data size (currently 12)
        // [1-2] Team Number (unsigned 16 bit value)
        // [2] Driver Station Number
        // [3] Controls Status Byte (Layout below)
        // [4] Contol Status Byte 2
        // [5-13] Battery Voltage (double value)
        public static void PackDriverStationReportData(this KeyValuePair<AllianceStation, IDriverStationReport> report,
            ref WireEncoder encoder)
        {
            IDriverStationReport value = report.Value;

            encoder.Write8(DriverStationReportSize);

            encoder.Write16((ushort)value.TeamNumber);
            encoder.Write8(value.Station.GetByte());

            encoder.Write8(value.GetControlByte1ToSend());
            encoder.Write8(value.GetControlByte2ToSend());
            encoder.WriteDouble(value.RobotBattery);
        }

        internal static byte GetControlByte2ToSend(this IDriverStationReport report)
        {
            byte controlByte = 0;
            if (report.IsBypassed) controlByte |= 0x01;
            return controlByte;
        }

        internal static byte GetControlByte1ToSend(this IDriverStationReport report)
        {
            byte controlByte = 0;
            if (report.DriverStationConnected) controlByte |= 0x01;
            if (report.RoboRioConnected) controlByte |= 0x02;
            if (report.IsBeingSentEnabled) controlByte |= 0x04;
            if (report.IsBeingSentAutonomous) controlByte |= 0x08;
            if (report.IsBeingSentEStopped) controlByte |= 0x10;
            if (report.IsReceivingEnabled) controlByte |= 0x20;
            if (report.IsReceivingAutonomous) controlByte |= 0x40;
            if (report.IsReceivingEStopped) controlByte |= 0x80;
            return controlByte;
        }

        public static IReadOnlyDictionary<AllianceStation, IDriverStationReport> GetDriverStationReports(this byte[] value)
        {
            MemoryStream stream = new MemoryStream(value);
            if (stream.Length < 2)
                return null;

            WireDecoder decoder = new WireDecoder(stream ,NetworkingConstants.NetworkTablesVersion);



            byte type = 0;
            decoder.Read8(ref type);
            if (type != (byte) CustomNetworkTableType.DriverStationReport)
                return null;
            byte count = 0;
            decoder.Read8(ref count);

            var reports = new Dictionary<AllianceStation, IDriverStationReport>(count);

            for (int i = 0; i < count; i++)
            {
                decoder.GetDriverStationReport(reports);
            }

            return reports;
        }

        public static void GetDriverStationReport(this WireDecoder decoder,
            IDictionary<AllianceStation, IDriverStationReport> reports)
        {
            // Ensure we have another 14 bytes.
            if (!decoder.HasMoreBytes(DriverStationReportSize + 1))
                return;

            byte size = 0;
            decoder.Read8(ref size);
            if (size != DriverStationReportSize)
                return;
            ushort teamNumber = 0;
            decoder.Read16(ref teamNumber);
            byte station = 0;
            decoder.Read8(ref station);
            byte controlByte = 0;
            decoder.Read8(ref controlByte);
            byte controlByte2 = 0;
            decoder.Read8(ref controlByte2);
            double battery = 0;
            decoder.ReadDouble(ref battery);

            AllianceStation sta = new AllianceStation(station);

            bool driverStationConnected = (controlByte & 0x01) == 0x01;
            bool roboRioConnected = (controlByte & 0x02) == 0x02;
            bool isBeingSentEnabled = (controlByte & 0x04) == 0x04;
            bool isBeingSentAutonomous = (controlByte & 0x08) == 0x08;
            bool isBeingSentEStopped = (controlByte & 0x10) == 0x10;
            bool isReceivingEnabled = (controlByte & 0x20) == 0x20;
            bool isReceivingAutonomous = (controlByte & 0x40) == 0x40;
            bool isReceivingEStopped = (controlByte & 0x80) == 0x80;
            bool isBypassed = (controlByte2 & 0x01) == 0x01;

            DriverStationReport report = new DriverStationReport(teamNumber, sta, driverStationConnected,
                roboRioConnected, isBeingSentEnabled, isBeingSentAutonomous, isBeingSentEStopped, isReceivingEnabled,
                isReceivingAutonomous, isReceivingEStopped, isBypassed, battery);

            reports.Add(sta, report);
        }
    }
}
