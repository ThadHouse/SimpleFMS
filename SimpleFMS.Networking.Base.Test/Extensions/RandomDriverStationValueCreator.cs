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
                byte randomControlByte = (byte)random.Next();
                byte randomControlByte2 = (byte)random.Next();
                ushort randomTeamNumber = (ushort)random.Next();
                double randomBattery = random.NextDouble();
                var randomStation = new AllianceStation((byte)i);

                DriverStationReport report = new DriverStationReport
                {
                    TeamNumber = randomTeamNumber,
                    RobotBattery = randomBattery,
                    Station = randomStation
                };
                randomControlByte.ReadControlByte1(ref report);
                randomControlByte2.ReadControlByte2(ref report);

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
                ushort randomTeamNumber = (ushort)random.Next();
                var randomStation = new AllianceStation((byte)i);
                bool randomBypass = random.Next(0, 1) != 0;

                DriverStationConfiguration config = new DriverStationConfiguration
                {
                    TeamNumber = randomTeamNumber,
                    Station = randomStation,
                    IsBypassed = randomBypass
                };

                configurations.Add(config);
            }
            return configurations;
        }
    }
}
