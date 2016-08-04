using System.Collections.Generic;
using System.IO;
using NetworkTables;
using NetworkTables.Wire;
using NUnit.Framework;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.Base.Networking;
using SimpleFMS.Networking.Base.Extensions;

namespace SimpleFMS.Networking.Base.Test.Extensions
{
    [TestFixture]
    public class DriverStationReportExtensionTests
    {
        [Test]
        public void IndividualPackedDataTransaction()
        {
            var station = new AllianceStation(AllianceStationSide.Red, AllianceStationNumber.Station3);

            DriverStationReport report = new DriverStationReport
            {
                Station = station,
                DriverStationConnected = true,
                IsBeingSentEStopped = false,
                IsReceivingEStopped = true,
                IsBeingSentEnabled = true,
                IsReceivingAutonomous = false,
                IsReceivingEnabled = false,
                IsBeingSentAutonomous = true,
                TeamNumber = 4488,
                RobotBattery = 7.58,
                RoboRioConnected = false
            };



            var pair = new KeyValuePair<AllianceStation, IDriverStationReport>(station, report);

            WireEncoder encoder = new WireEncoder(NetworkingConstants.NetworkTablesVersion);
            pair.PackDriverStationReportData(ref encoder);

            byte[] data = encoder.Buffer;
            Assert.That(data, Has.Length.EqualTo(13));

            Dictionary<AllianceStation, IDriverStationReport> reportDictionary =
                new Dictionary<AllianceStation, IDriverStationReport>(1);

            MemoryStream stream = new MemoryStream(data);
            WireDecoder decoder = new WireDecoder(stream, NetworkingConstants.NetworkTablesVersion);
            decoder.GetDriverStationReport(reportDictionary);



            Assert.That(reportDictionary, Has.Count.EqualTo(1));
            Assert.That(reportDictionary, Contains.Key(station));
            Assert.That(reportDictionary, Contains.Value(report));
        }

        [Test]
        public void DictionaryPackedDataTransaction([Range(0, 6)] int count)
        {
            var reports = RandomDriverStationValueCreator.GetRandomDriverStationReports(count);

            var data = reports.PackDriverStationReportData();

            int byteCount = count * 13 + 2;

            Assert.That(data, Has.Length.EqualTo(byteCount));

            var returnedReports = data.GetDriverStationReports();

            Assert.That(returnedReports, Has.Count.EqualTo(count));

            Assert.That(returnedReports, Is.EquivalentTo(reports));
        }
    }
}
