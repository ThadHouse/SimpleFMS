using System;
using System.Collections;
using NUnit.Framework;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.Base.Test
{
    public class AllStationEnums : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var side in Enum.GetValues(typeof(AllianceStationSide)))
            {
                foreach (var number in Enum.GetValues(typeof(AllianceStationNumber)))
                {
                        yield return new[] { side, number };
                }
            }
        }
    }

    [TestFixture]
    public class AllianceStationTests
    {
        [TestCaseSource(typeof(AllStationEnums))]
        public void AllianceStationCreateFromSeperateEnums(AllianceStationSide side, AllianceStationNumber number)
        {
            var station = new AllianceStation(side, number);
            var station2 = new AllianceStation(side, number);

            Assert.That(station == station2);
            Assert.That(station2 == station);
            Assert.That(station.Equals(station2));
            Assert.That(station2.Equals(station));

            Assert.That(((object)station).Equals(station2));
        }

        [TestCaseSource(typeof(AllStationEnums))]
        public void AllianceStationCreatedFromByte(AllianceStationSide side, AllianceStationNumber number)
        {
            var station = new AllianceStation(side, number);
            var station2 = new AllianceStation(station.GetByte());

            Assert.That(station == station2);
            Assert.That(station2 == station);
            Assert.That(station.Equals(station2));
            Assert.That(station2.Equals(station));

            Assert.That(((object)station).Equals(station2));
        }
    }
}
