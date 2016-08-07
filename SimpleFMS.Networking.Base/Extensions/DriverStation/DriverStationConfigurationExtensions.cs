using System;
using System.Collections.Generic;
using System.IO;
using NetworkTables.Wire;
using SimpleFMS.Base.DriverStation;
using SimpleFMS.Base.Enums;
using SimpleFMS.Base.Networking;

namespace SimpleFMS.Networking.Base.Extensions.DriverStation
{
    public static class DriverStationConfigurationExtensions
    {
        private const int DriverStationConfigurationSize = 4;

        public static byte[] PackDriverStationSetConfigurationResponse(bool set)
        {
            return new [] {(byte) CustomNetworkTableType.DriverStationConfiguration, (byte) (set ? 1 : 0)};
        }

        public static bool UnpackDriverStationSetConfigurationResponse(this byte[] value)
        {
            if (value.Length < 2)
                return false;
            if (value[0] != (byte)CustomNetworkTableType.DriverStationConfiguration)
                return false;
            return value[1] != 0;
        }

        public static byte[] PackDriverStationConfigurationData(
            this IReadOnlyList<IDriverStationConfiguration> configurations, int matchNumber, MatchType matchType)
        {
            if (configurations.Count > 6)
                throw new ArgumentOutOfRangeException(nameof(configurations),
                    $"Only a maximum of {AllianceStation.MaxNumAllianceStations}");

            WireEncoder encoder = new WireEncoder(NetworkingConstants.NetworkTablesVersion);

            encoder.Write8((byte)CustomNetworkTableType.DriverStationConfiguration);
            encoder.Write16((ushort)matchNumber);
            encoder.Write8((byte)matchType);
            encoder.Write8((byte)configurations.Count);

            foreach (var configuration in configurations)
            {
                configuration.PackDriverStationConfigurationData(ref encoder);
            }

            return encoder.Buffer;
        }

        public static void PackDriverStationConfigurationData(this IDriverStationConfiguration configuration,
            ref WireEncoder encoder)
        {
            encoder.Write8(DriverStationConfigurationSize);

            encoder.Write16((ushort)configuration.TeamNumber);
            encoder.Write8(configuration.Station.GetByte());
            encoder.Write8((byte)(configuration.IsBypassed ? 1 : 0));
        }

        public static IReadOnlyList<IDriverStationConfiguration> GetDriverStationConfigurations(this byte[] value, 
            out int matchNumber, out MatchType matchType)
        {
            matchNumber = 0;
            matchType = 0;

            if (value.Length < 5)
                return null;

            MemoryStream stream = new MemoryStream(value);

            WireDecoder decoder = new WireDecoder(stream, NetworkingConstants.NetworkTablesVersion);



            byte type = 0;
            decoder.Read8(ref type);
            if (type != (byte) CustomNetworkTableType.DriverStationConfiguration)
            {
                matchNumber = 0;
                matchType = 0;
                return null;
            }
            ushort mNum = 0;
            decoder.Read16(ref mNum);
            matchNumber = mNum;
            byte mType = 0;
            decoder.Read8(ref mType);
            matchType = (MatchType) mType;
            byte count = 0;
            decoder.Read8(ref count);

            var configurations = new List<IDriverStationConfiguration>(count);

            for (int i = 0; i < count; i++)
            {
                decoder.GetDriverStationConfiguration(configurations);
            }

            return configurations;
        }

        public static void GetDriverStationConfiguration(this WireDecoder decoder,
            IList<IDriverStationConfiguration> configurations)
        {
            // Ensure we have another 5 bytes
            if (!decoder.HasMoreBytes(DriverStationConfigurationSize + 1))
                return;

            byte size = 0;
            decoder.Read8(ref size);
            if (size != DriverStationConfigurationSize)
                return;
            ushort teamNumber = 0;
            decoder.Read16(ref teamNumber);
            byte station = 0;
            decoder.Read8(ref station);
            byte bypass = 0;
            decoder.Read8(ref bypass);

            DriverStationConfiguration config = new DriverStationConfiguration((short)teamNumber, new AllianceStation(station),
                bypass != 0);

            configurations.Add(config);
        }
    }
}
