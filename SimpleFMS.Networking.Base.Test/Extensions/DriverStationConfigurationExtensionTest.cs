using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables.Wire;
using NUnit.Framework;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.Base.Networking;
using SimpleFMS.Networking.Base.Extensions;

namespace SimpleFMS.Networking.Base.Test.Extensions
{
    [TestFixture]
    public class DriverStationConfigurationExtensionTest
    {
        [Test]
        public void IndividualPackedDataTransaction()
        {
            WireEncoder encoder = new WireEncoder(NetworkingConstants.NetworkTablesVersion);

            DriverStationConfiguration configuration = new DriverStationConfiguration
            {
                Station = new AllianceStation(AllianceStationSide.Red, AllianceStationNumber.Station2),
                TeamNumber = 4488,
                IsBypassed = true
            };

            configuration.PackDriverStationConfigurationData(ref encoder);

            byte[] data = encoder.Buffer;
            Assert.That(data, Has.Length.EqualTo(5));

            List<IDriverStationConfiguration> configurations = new List<IDriverStationConfiguration>(1);

            MemoryStream stream = new MemoryStream(data);
            WireDecoder decoder = new WireDecoder(stream, NetworkingConstants.NetworkTablesVersion);
            decoder.GetDriverStationConfiguration(configurations);

            Assert.That(configurations, Has.Count.EqualTo(1));
            Assert.That(configurations, Contains.Item(configuration));
        }

        [Test]
        public void ListPackedDataTransaction([Range(0, 6)] int count)
        {
            int mNum = 0;
            MatchType mType = 0;

            var configurations = RandomDriverStationValueCreator.GetRandomDriverStationConfigurations(count, ref mNum, ref mType);

            var data = configurations.PackDriverStationConfigurationData(mNum, mType);

            int byteCount = count * 5 + 5;
            Assert.That(data, Has.Length.EqualTo(byteCount));

            int matchNum = 0;
            MatchType matchType = 0;
            var returns = data.GetDriverStationConfigurations(out matchNum, out matchType);

            Assert.That(returns, Has.Count.EqualTo(count));
            Assert.That(matchNum, Is.EqualTo(mNum));
            Assert.That(matchType, Is.EqualTo(mType));
            Assert.That(returns, Is.EquivalentTo(configurations));
        }
    }
}
