using System;
using System.Collections.Generic;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.DriverStation.Interfaces;
using SimpleFMS.Networking.Base.Extensions;

namespace SimpleFMS.Networking.Base.Test.Extensions
{
    public static class RandomDriverStationReportDictionaryCreator
    {

        public static IReadOnlyDictionary<AllianceStation, IDriverStationReport> GetRandomDriverStationReports(int count)
        {
            Dictionary<AllianceStation, IDriverStationReport> reports = new Dictionary<AllianceStation, IDriverStationReport>(count);

            Random random = new Random();

            for (int i = 0; i < count; i++)
            {
                byte randomControlByte = (byte)random.Next();
                ushort randomTeamNumber = (ushort)random.Next();
                double randomBattery = random.NextDouble();
                var randomStation = new AllianceStation((byte)i);

                DriverStationReport report = new DriverStationReport
                {
                    TeamNumber = randomTeamNumber,
                    RobotBattery = randomBattery,
                    Station = randomStation
                };
                randomControlByte.ReadControlByte(ref report);

                reports.Add(randomStation, report);
            }
            return reports;
        }
    }
}
