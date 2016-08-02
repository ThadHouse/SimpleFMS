using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkTables.Wire;
using SimpleFMS.DriverStation.Base.Enums;
using SimpleFMS.DriverStation.Base.Interfaces;
using SimpleFMS.DriverStation.Base.Tuples;

namespace SimpleFMS.NetworkTables
{
    public class RequestedDriverStationStructure : ISendPacker
    {
        public IReadOnlyDictionary<ImmutableStructTuple<AllianceStationSide, AllianceStationNumber>, int>
            RequestedDriverStations { get; set; }

        public byte[] PackData()
        {
            if (RequestedDriverStations.Count > 6) return null;

            WireEncoder encoder = new WireEncoder(0x0300);
            encoder.Write8((byte)DriverStationDataValues.RequestedStructArray);
            encoder.Write8((byte) RequestedDriverStations.Count);

            foreach (var requestedDriverStation in RequestedDriverStations)
            {
                encoder.Write8((byte)DriverStationDataValues.RequestedIndividualStruct);
                encoder.Write16((ushort)requestedDriverStation.Value);
                encoder.Write8((byte)requestedDriverStation.Key.First);
                encoder.Write8((byte)requestedDriverStation.Key.Second);
            }

            return encoder.Buffer;
        }
    }
}
