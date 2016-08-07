using System;
using System.Collections.Generic;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.Base.DriverStation
{
    /// <summary>
    /// Represents an alliance station on the field
    /// </summary>
    public struct AllianceStation : IEquatable<AllianceStation>
    {
        /// <summary>
        /// The maximum number of alliance stations on the field
        /// </summary>
        public static readonly int MaxNumAllianceStations = GetAllAllianceStations().Count;

        /// <summary>
        /// Gets a list of all the alliance stations on the field
        /// </summary>
        /// <returns>A list of all alliance stations on the field</returns>
        public static IReadOnlyList<AllianceStation> GetAllAllianceStations()
        {
            List<AllianceStation> stations = new List<AllianceStation>(6);
            foreach (AllianceStationSide side in Enum.GetValues(typeof(AllianceStationSide)))
            {
                foreach (AllianceStationNumber number in Enum.GetValues(typeof(AllianceStationNumber)))
                {
                    stations.Add(new AllianceStation(side, number));
                }
            }
            return stations;
        }

        /// <inheritdoc/>
        public bool Equals(AllianceStation other)
        {
            return AllianceSide == other.AllianceSide && StationNumber == other.StationNumber;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AllianceStation && Equals((AllianceStation) obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) AllianceSide*397) ^ (int) StationNumber;
            }
        }

        /// <summary>
        /// Checks to see if two <see cref="AllianceStation">Alliance Stations</see>
        /// are equal
        /// </summary>
        /// <param name="left">The left operator</param>
        /// <param name="right">The right operator</param>
        /// <returns>True if the two values are equal</returns>
        public static bool operator ==(AllianceStation left, AllianceStation right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Checks to see if two <see cref="AllianceStation">Alliance Stations</see>
        /// are not equal
        /// </summary>
        /// <param name="left">The left operator</param>
        /// <param name="right">The right operator</param>
        /// <returns>True if the two values are not equal</returns>
        public static bool operator !=(AllianceStation left, AllianceStation right)
        {
            return !left.Equals(right);
        }


        /// <summary>
        /// Gets the station side associated with this station
        /// </summary>
        public AllianceStationSide AllianceSide { get; }
        /// <summary>
        /// Gets the station number associated with this station
        /// </summary>
        public AllianceStationNumber StationNumber { get; }

        /// <summary>
        /// Creates a new alliance station from an alliance and station number
        /// </summary>
        /// <param name="side">The side the alliance is on</param>
        /// <param name="number">The number of the alliance station</param>
        public AllianceStation(AllianceStationSide side, AllianceStationNumber number)
        {
            AllianceSide = side;
            StationNumber = number;
        }

        /// <summary>
        /// Creates a new alliance station from a raw station number
        /// </summary>
        /// <param name="combined"></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the combined
        /// value is greater then the <see cref="MaxNumAllianceStations">
        /// Max number of alliance stations</see></exception>
        public AllianceStation(byte combined)
        {
            if (combined > MaxNumAllianceStations)
                throw new ArgumentOutOfRangeException(nameof(combined), combined, 
                    $"Value must be between 0 and {MaxNumAllianceStations}");
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

        /// <summary>
        /// Gets the raw byte from this alliance station to send over the network
        /// </summary>
        /// <returns></returns>
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
