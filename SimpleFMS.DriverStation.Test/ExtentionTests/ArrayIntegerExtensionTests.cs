using System;
using System.Net;
using NUnit.Framework;
using SimpleFMS.DriverStation.Extensions;

namespace SimpleFMS.DriverStation.Test.ExtentionTests
{
    [TestFixture]
    public class ArrayIntegerExtensionTests
    {
        [OneTimeSetUp]
        public void TestForLittleEndian()
        {
            Assert.That(BitConverter.IsLittleEndian, Is.True);
        }

        [Test]
        public void IntToByteArray()
        {
            int team = 4488;
            byte[] extensionValue = new byte[4];
            int index = 0;
            team.AddToArray(extensionValue, ref index);

            byte[] baseLib = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(team));

            Assert.That(index, Is.EqualTo(4));
            Assert.That(extensionValue, Is.EqualTo(baseLib));
        }

        [Test]
        public void UIntToByteArray()
        {
            uint team = 4488;
            byte[] extensionValue = new byte[4];
            int index = 0;
            team.AddToArray(extensionValue, ref index);

            byte[] baseLib = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)team));

            Assert.That(index, Is.EqualTo(4));
            Assert.That(extensionValue, Is.EqualTo(baseLib));
        }

        [Test]
        public void ShortToByteArray()
        {
            short team = 4488;
            byte[] extensionValue = new byte[2];
            int index = 0;
            team.AddToArray(extensionValue, ref index);

            byte[] baseLib = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(team));

            Assert.That(index, Is.EqualTo(2));
            Assert.That(extensionValue, Is.EqualTo(baseLib));
        }

        [Test]
        public void UShortToByteArray()
        {
            ushort team = 4488;
            byte[] extensionValue = new byte[2];
            int index = 0;
            team.AddToArray(extensionValue, ref index);

            byte[] baseLib = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)team));

            Assert.That(index, Is.EqualTo(2));
            Assert.That(extensionValue, Is.EqualTo(baseLib));
        }



        [Test]
        public void ByteArrayToInt()
        {
            int team = 4488;
            byte[] baseLib = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(team));
            Assert.That(baseLib, Has.Length.EqualTo(4));


            int index = 0;
            int convertedTeam = baseLib.GetInt(ref index);

            Assert.That(index, Is.EqualTo(4));
            Assert.That(convertedTeam, Is.EqualTo(team));

        }

        [Test]
        public void ByteArrayToUInt()
        {
            uint team = 4488;
            byte[] baseLib = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)team));
            Assert.That(baseLib, Has.Length.EqualTo(4));


            int index = 0;
            uint convertedTeam = baseLib.GetUInt(ref index);

            Assert.That(index, Is.EqualTo(4));
            Assert.That(convertedTeam, Is.EqualTo(team));
        }

        [Test]
        public void ByteArrayToShort()
        {
            short team = 4488;
            byte[] baseLib = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(team));
            Assert.That(baseLib, Has.Length.EqualTo(2));


            int index = 0;
            short convertedTeam = baseLib.GetShort(ref index);

            Assert.That(index, Is.EqualTo(2));
            Assert.That(convertedTeam, Is.EqualTo(team));
        }

        [Test]
        public void ByteArrayToUShort()
        {
            ushort team = 4488;
            byte[] baseLib = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)team));
            Assert.That(baseLib, Has.Length.EqualTo(2));


            int index = 0;
            ushort convertedTeam = baseLib.GetUShort(ref index);

            Assert.That(index, Is.EqualTo(2));
            Assert.That(convertedTeam, Is.EqualTo(team));
        }
    }
}
