using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleFMS.Base.DriverStation.Enums;
using SimpleFMS.DriverStation.Extensions;
using SimpleFMS.DriverStation.Test.TcpDataTests;
using SimpleFMS.DriverStation.UdpData;

namespace SimpleFMS.DriverStation.Test.UdpDataTests
{
    public class AllReceiveControlBytes : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var auto in new[]{true, false})
            {
                foreach (var enabled in new[] { true, false })
                {
                    foreach (var eStopped in new[] { true, false })
                    {
                        foreach (var hasComms in new [] {true, false})
                        {
                            foreach (var teamNumber in DriverStationConnectionStatusDataTests.TeamNumbersToTest)
                            {
                                yield return
                                    new[]
                                    {
                                        auto, enabled, eStopped, hasComms, teamNumber
                                    };
                            }
                        }
                    }
                }
            }
        }
    }

    [TestFixture]
    public class DriverStationStatusDataTests
    {
        [TestCaseSource(typeof(AllReceiveControlBytes))]
        public void StatusDataParse(bool auto, bool enabled, bool eStopped, bool hasComms, int teamNumber)
        {
            byte controlWord = GetControlWord(auto, enabled, hasComms, eStopped);
            Random random = new Random();
            ushort packetNumber = (ushort) random.Next();
            byte[] packet = new byte[8];
            int index = 0;
            packetNumber.AddToArray(packet, ref index);
            packet[2] = 0;
            packet[3] = controlWord;
            index = 4;
            ((ushort)teamNumber).AddToArray(packet, ref index);
            ushort batteryVoltage = (ushort) random.Next();
            batteryVoltage.AddToArray(packet, ref index);

            DriverStationStatusData data = new DriverStationStatusData();
            var status = data.ParseData(packet);

            Assert.That(status, Is.EqualTo(ReceiveParseStatus.TrackedPacketValid));
            Assert.That(data.HasRobotComms, Is.EqualTo(hasComms));
            Assert.That(data.IsEnabled, Is.EqualTo(enabled));
            Assert.That(data.IsEStopped, Is.EqualTo(eStopped));
            Assert.That(data.IsAutonomous, Is.EqualTo(auto));
            Assert.That(data.TeamNumber, Is.EqualTo(teamNumber));
            Assert.That(data.PacketNumber, Is.EqualTo(packetNumber));
            Assert.That(data.RobotBattery, Is.EqualTo(batteryVoltage/256.0).Within(0.1));
        }


        [Test]
        public void StatusDataParseTooShort()
        {
            byte[] packet = new byte[2];
            DriverStationStatusData data = new DriverStationStatusData();
            var result = data.ParseData(packet);
            Assert.That(result, Is.EqualTo(ReceiveParseStatus.InvalidPacket));
        }

        [Test]
        public void StatusDataParseNull()
        {
            DriverStationStatusData data = new DriverStationStatusData();
            var result = data.ParseData(null);
            Assert.That(result, Is.EqualTo(ReceiveParseStatus.InvalidPacket));
        }

        private byte GetControlWord(bool auto, bool enabled, bool robotComms, bool eStopped)
        {
            byte controlWord = 0;
            unchecked
            {
                if (auto) controlWord |= 2;
                else controlWord &= (byte)~2;
                if (enabled) controlWord |= 4;
                else controlWord &= (byte)~4;
                if (robotComms) controlWord |= 32;
                else controlWord &= (byte)~32;
                if (eStopped) controlWord |= 128;
                else controlWord &= (byte)~128;
            }
            return controlWord;
        }
    }
}
