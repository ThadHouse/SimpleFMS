using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleFMS.Base.DriverStation.Enums;
using SimpleFMS.DriverStation.TcpData;

namespace SimpleFMS.DriverStation.Test.TcpDataTests
{
    [TestFixture]
    public class DriverStationConnectionStatusDataTests
    {
        public static readonly object[] TeamNumbersToTest =
        {
            125,
            254,
            422,
            469,
            1058,
            1114,
            1241,
            1540,
            1676,
            1747,
            2451,
            3467,
            3476,
            4488
        };


        [TestCaseSource(nameof(TeamNumbersToTest))]
        public void TestStatusDataReceiveTeamNumber(int teamNumber)
        {
            byte[] teamNumberBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)teamNumber));
            byte[] testData = {0, 3, (byte) DriverStationConnectionPacketType.SetTeam, teamNumberBytes[0], teamNumberBytes[1]};

            DriverStationConnectionStatusData connData = new DriverStationConnectionStatusData();
            var result = connData.ParseData(testData);

            Assert.That(result, Is.EqualTo(ReceiveParseStatus.TrackedPacketValid));
            Assert.That(connData.Status, Is.EqualTo(DriverStationConnectionPacketType.SetTeam));
            Assert.That(connData.TeamNumber, Is.EqualTo(teamNumber));
        }

        [Test]
        public void TestStatusDataReceivePing()
        {
            byte[] testData = { 0, 3, (byte)DriverStationConnectionPacketType.Ping, 0};

            DriverStationConnectionStatusData connData = new DriverStationConnectionStatusData();
            var result = connData.ParseData(testData);

            Assert.That(result, Is.EqualTo(ReceiveParseStatus.TrackedPacketValid));
            Assert.That(connData.Status, Is.EqualTo(DriverStationConnectionPacketType.Ping));
        }

        [Test]
        public void TestStatusDataReceiveTooSmall()
        {
            byte[] testData = { 0, 3 };

            DriverStationConnectionStatusData connData = new DriverStationConnectionStatusData();
            var result = connData.ParseData(testData);

            Assert.That(result, Is.EqualTo(ReceiveParseStatus.InvalidPacket));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestStatusDataReceiveTeamNumberTooSmall(bool threeBytes)
        {
            var testData = threeBytes
                ? new byte[] {0, 3, (byte) DriverStationConnectionPacketType.SetTeam}
                : new byte[] {0, 3, (byte) DriverStationConnectionPacketType.SetTeam, 25};

            DriverStationConnectionStatusData connData = new DriverStationConnectionStatusData();
            var result = connData.ParseData(testData);

            Assert.That(result, Is.EqualTo(ReceiveParseStatus.TrackedPacketInvalid));
        }


    }
}
