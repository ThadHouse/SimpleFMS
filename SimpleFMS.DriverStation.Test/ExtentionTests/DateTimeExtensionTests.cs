using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SimpleFMS.DriverStation.Extensions;

namespace SimpleFMS.DriverStation.Test.ExtentionTests
{
    [TestFixture]
    public class DateTimeExtensionTests
    {
        public static  DateTime GetDateTimeForTest()
        {
            return new DateTime(2016, 3, 21, 8, 38, 8, 578);
        }

        public static readonly byte[] DateTimeResult = { 0, 8, 209, 208, 8, 38, 8, 21, 3, 116 };


        [Test]
        public void DateTimeToArray()
        {
            var time = GetDateTimeForTest();
            byte[] data = new byte[10];
            int index = 0;
            time.AddToArray(data, ref index);
            Assert.That(index, Is.EqualTo(10));
            Assert.That(data, Is.EquivalentTo(DateTimeResult));
        }

        [Test]
        public void DateTimeToArrayTooSmall()
        {
            byte[] data = new byte[2];
            int index = 0;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                GetDateTimeForTest().AddToArray(data, ref index);
            });
        }

        [Test]
        public void DateTimeArrayNull()
        {
            int index = 0;
            Assert.Throws<ArgumentNullException>(() =>
            {
                GetDateTimeForTest().AddToArray(null, ref index);
            });
        }

        [Test]
        public void DataTimeArrayIndexTooLarge()
        {

            byte[] data = new byte[8];
            int index = 5;
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                GetDateTimeForTest().AddToArray(data, ref index);
            });
        }

    }
}
