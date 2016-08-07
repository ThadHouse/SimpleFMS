using System;
using System.Collections.Generic;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.Networking.Base.Extensions.DriverStation;

namespace SimpleFMS.Networking.Base.Test.Extensions
{
    public static class RandomDriverStationValueCreator
    {

        public static IReadOnlyDictionary<AllianceStation, IDriverStationReport> GetRandomDriverStationReports(int count)
        {
            Dictionary<AllianceStation, IDriverStationReport> reports = new Dictionary<AllianceStation, IDriverStationReport>(count);

            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                byte controlByte = (byte)random.Next();
                byte controlByte2 = (byte)random.Next();
                ushort randomTeamNumber = (ushort)random.Next();
                double randomBattery = random.NextDouble();
                var randomStation = new AllianceStation((byte)i);

                bool driverStationConnected = (controlByte & 0x01) == 0x01;
                bool roboRioConnected = (controlByte & 0x02) == 0x02;
                bool isBeingSentEnabled = (controlByte & 0x04) == 0x04;
                bool isBeingSentAutonomous = (controlByte & 0x08) == 0x08;
                bool isBeingSentEStopped = (controlByte & 0x10) == 0x10;
                bool isReceivingEnabled = (controlByte & 0x20) == 0x20;
                bool isReceivingAutonomous = (controlByte & 0x40) == 0x40;
                bool isReceivingEStopped = (controlByte & 0x80) == 0x80;
                bool isBypassed = (controlByte2 & 0x01) == 0x01;

                DriverStationReport report = new DriverStationReport(randomTeamNumber, randomStation, driverStationConnected,
                    roboRioConnected, isBeingSentEnabled, isBeingSentAutonomous, isBeingSentEStopped, isReceivingEnabled,
                    isReceivingAutonomous, isReceivingEStopped, isBypassed, randomBattery);

                reports.Add(randomStation, report);
            }
            return reports;
        }

        public static IReadOnlyList<IDriverStationConfiguration> GetRandomDriverStationConfigurations(int count,
            ref int matchNumber, ref MatchType matchType)
        {
            List<IDriverStationConfiguration> configurations = new List<IDriverStationConfiguration>(count);

            Random random = new Random();

            matchNumber = (ushort)random.Next();
            matchType = (MatchType)random.Next(0, 3);

            for (int i = 0; i < count; i++)
            {
                short randomTeamNumber = (short)random.Next(0, short.MaxValue);
                var randomStation = new AllianceStation((byte)i);
                bool randomBypass = random.Next(0, 1) != 0;

                DriverStationConfiguration config = new DriverStationConfiguration(randomTeamNumber, randomStation,
                    randomBypass);

                configurations.Add(config);
            }
            return configurations;
        }
    }
}
