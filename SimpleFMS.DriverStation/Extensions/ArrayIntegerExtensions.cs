using System;
using System.Runtime.CompilerServices;

namespace SimpleFMS.DriverStation.Extensions
{
    // Note these are all little to big endian, so this software will not work on
    // Big endian systems without changing the source code
    internal static class ArrayIntegerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddToArray(this int input, byte[] array, ref int index)
        {
            if (array.Length < index + 4)
            {
                throw new ArgumentOutOfRangeException(nameof(array), "The destination array was not long enough.");
            }

            array[index + 3] = (byte)(input & 0xff);
            array[index + 2] = (byte)((input >> 8) & 0xff);
            array[index + 1] = (byte)((input >> 16) & 0xff);
            array[index + 0] = (byte)((input >> 24) & 0xff);
            index += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddToArray(this uint input, byte[] array, ref int index)
        {
            if (array.Length < index + 4)
            {
                throw new ArgumentOutOfRangeException(nameof(array), "The destination array was not long enough.");
            }

            array[index + 3] = (byte)(input & 0xff);
            array[index + 2] = (byte)((input >> 8) & 0xff);
            array[index + 1] = (byte)((input >> 16) & 0xff);
            array[index + 0] = (byte)((input >> 24) & 0xff);
            index += 4;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddToArray(this short input, byte[] array, ref int index)
        {
            if (array.Length < index + 2)
            {
                throw new ArgumentOutOfRangeException(nameof(array), "The destination array was not long enough.");
            }

            array[index + 1] = (byte)(input & 0xff);
            array[index] = (byte)((input >> 8) & 0xff);
            index += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddToArray(this ushort input, byte[] array, ref int index)
        {
            if (array.Length < index + 2)
            {
                throw new ArgumentOutOfRangeException(nameof(array), "The destination array was not long enough.");
            }

            array[index + 1] = (byte)(input & 0xff);
            array[index] = (byte)((input >> 8) & 0xff);
            index += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetInt(this byte[] input, ref int index)
        {
            if (input.Length < index + 4)
            {
                throw new ArgumentOutOfRangeException(nameof(input), "The destination array was not long enough.");
            }
            int data = input[index++];
            data <<= 8;
            data += input[index++];
            data <<= 8;
            data += input[index++];
            data <<= 8;
            data += input[index++];
            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetUInt(this byte[] input, ref int index)
        {
            if (input.Length < index + 4)
            {
                throw new ArgumentOutOfRangeException(nameof(input), "The destination array was not long enough.");
            }

            uint data = input[index++];
            data <<= 8;
            data += input[index++];
            data <<= 8;
            data += input[index++];
            data <<= 8;
            data += input[index++];
            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static short GetShort(this byte[] input, ref int index)
        {
            if (input.Length < index + 2)
            {
                throw new ArgumentOutOfRangeException(nameof(input), "The destination array was not long enough.");
            }

            short data = input[index++];
            data <<= 8;
            data += input[index++];
            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetUShort(this byte[] input, ref int index)
        {
            if (input.Length < index + 2)
            {
                throw new ArgumentOutOfRangeException(nameof(input), "The destination array was not long enough.");
            }

            ushort data = input[index++];
            data <<= 8;
            data += input[index++];
            return data;
        }
    }
}
