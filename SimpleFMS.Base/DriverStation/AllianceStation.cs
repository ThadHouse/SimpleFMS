using System;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.Base.DriverStation
{
    public static class AllianceStationConstants
    {
        public static readonly int MaxNumDriverStations = GetMaxNumDriverStations();

        private static int GetMaxNumDriverStations()
        {
            int count = 0;
            foreach (var side in Enum.GetValues(typeof(AllianceStationSide)))
            {
                foreach (var number in Enum.GetValues(typeof(AllianceStationNumber)))
                {
                    count++;
                }
            }
            return count;
        }
    }

    public struct AllianceStation : IEquatable<AllianceStation>
    {
        public bool Equals(AllianceStation other)
        {
            return AllianceSide == other.AllianceSide && StationNumber == other.StationNumber;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AllianceStation && Equals((AllianceStation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) AllianceSide*397) ^ (int) StationNumber;
            }
        }

        public static bool operator ==(AllianceStation left, AllianceStation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AllianceStation left, AllianceStation right)
        {
            return !left.Equals(right);
        }

        public AllianceStationSide AllianceSide { get; }
        public AllianceStationNumber StationNumber { get; }

        public AllianceStation(AllianceStationSide side, AllianceStationNumber number)
        {
            AllianceSide = side;
            StationNumber = number;
        }

        public AllianceStation(byte combined)
        {
            if (combined > 5)
                throw new ArgumentOutOfRangeException(nameof(combined), combined, "Value must be between 0 and 5");
            if (combined >= 3)
            {
                // Blue Alliance
                AllianceSide = AllianceStationSide.Blue;
                StationNumber = (AllianceStationNumber) (combined - 3);
            }
            else
            {
                AllianceSide = AllianceStationSide.Red;
                StationNumber = (AllianceStationNumber) combined;
            }
        }

        public byte GetByte()
        {
            switch (AllianceSide)
            {
                case AllianceStationSide.Red:
                    return (byte)(StationNumber);
                case AllianceStationSide.Blue:
                    return (byte)(StationNumber + 3);
                default:
                    throw new ArgumentOutOfRangeException(nameof(AllianceSide), "Side invalid");
            }
        }
    }
}
