using System;
using System.Runtime.CompilerServices;

namespace SimpleFMS.DriverStation.Extensions
{
    /// <summary>
    /// Extensions to the DateTime class
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Adds a DateTime variable to a packet to be sent
        /// </summary>
        /// <param name="currentTime">The current time to add</param>
        /// <param name="array">The array to add to</param>
        /// <param name="index">The index of the array to start at</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddToArray(this DateTime currentTime, byte[] array, ref int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array), "Input array cannot be null");
            }

            // Make sure we have enough room in the array
            if (array.Length < index + 10)
            {
                throw new ArgumentOutOfRangeException(nameof(array), "The input array is not long enough.");
            }

            (currentTime.Millisecond * 1000).AddToArray(array, ref index);
            array[index] = (byte)currentTime.Second;
            index++;
            array[index] = (byte)currentTime.Minute;
            index++;
            array[index] = (byte)currentTime.Hour;
            index++;
            array[index] = (byte)currentTime.Day;
            index++;
            array[index] = (byte)currentTime.Month;
            index++;
            array[index] = (byte)(currentTime.Year - 1900);
            index++;
        }
    }
}
