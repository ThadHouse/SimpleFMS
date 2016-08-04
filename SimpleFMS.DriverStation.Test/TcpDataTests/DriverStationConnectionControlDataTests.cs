using System;
using System.Collections;
using NUnit.Framework;
using SimpleFMS.Base.Enums;
using SimpleFMS.DriverStation.Enums;
using SimpleFMS.DriverStation.TcpData;

namespace SimpleFMS.DriverStation.Test.TcpDataTests
{
    public class AllStationEnums : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var side in Enum.GetValues(typeof(AllianceStationSide)))
            {
                foreach (var number in Enum.GetValues(typeof(AllianceStationNumber)))
                {
                    foreach (var status in Enum.GetValues(typeof(AllianceStationStatus)))
                    {
                        yield return new[] {side, number, status};
                    }
                }
            }
        }
    }

    [TestFixture]
    public class DriverStationConnectionControlDataTests
    {


        [TestCaseSource(typeof(AllStationEnums))]
        public void TestControlPacketTypeSendTeamResponse(AllianceStationSide side, AllianceStationNumber number, AllianceStationStatus status)
        {
            byte allianceByte = 0;
            switch (side)
            {
                case AllianceStationSide.Red:
                    allianceByte = (byte)(number);
                    break;
                case AllianceStationSide.Blue:
                    allianceByte = (byte)(number + 3);
                    break;
            }

            byte[] correctBytes =
            {
                0, 3, (byte) DriverStationConnectionPacketType.SendTeamResponse, allianceByte,
                (byte) status
            };

            var packer = new DriverStationConnectionControlData
            {
                AllianceSide = side,
                StationNumber = number,
                StationStatus = status
            };

            byte[] packed = packer.PackData();

            Assert.That(packed, Has.Length.EqualTo(5));
            Assert.That(packed, Is.EquivalentTo(correctBytes));
        }
    }
}
