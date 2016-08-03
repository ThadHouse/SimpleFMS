using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NetworkTables.Wire;
using SimpleFMS.Base.DriverStation.Interfaces;
using SimpleFMS.Base.Tuples;

namespace SimpleFMS.NetworkTables
{
    public enum DriverStationDataValues
    {
        CommunicationIndividualStruct = 6,
        CommunicationStructArray = 8,

        RequestedIndividualStruct = 10,
        RequestedStructArray = 12
    }

    public class ConnectedDriverStationStructure : ISendPacker
    {
        public IReadOnlyList<ValueTuple<IDriverStationIncomingData, IDriverStationOutgoingData>> DriverStationData
        { get; set; }

        
        private byte[] PackIndividualStructure(ValueTuple<IDriverStationIncomingData, IDriverStationOutgoingData> data)
        {
            WireEncoder encoder = new WireEncoder(0x0300);
            var outgoing = data.Second;
            
            encoder.Write8((byte)DriverStationDataValues.CommunicationIndividualStruct);
            encoder.WriteString(outgoing.IpAddress.ToString());
            encoder.Write8((byte)outgoing.AllianceSide);
            encoder.Write8((byte)outgoing.StationNumber);
            encoder.Write8(BoolToByte(outgoing.IsEStopped));
            encoder.Write8(BoolToByte(outgoing.IsEnabled));
            encoder.Write8(BoolToByte(outgoing.IsAutonomous));

            var incoming = data.First;
            if (incoming == null)
            {
                for (int i = 0; i < 15; i++)
                {
                    encoder.Write8(0);
                }
            }
            else
            {
                encoder.Write8(BoolToByte(incoming.HasDriverStationComms));
                encoder.Write8(BoolToByte(incoming.HasRobotComms));
                encoder.Write8(BoolToByte(incoming.IsEStopped));
                encoder.Write8(BoolToByte(incoming.IsEnabled));
                encoder.Write8(BoolToByte(incoming.IsAutonomous));
                encoder.Write16((ushort) incoming.TeamNumber);
                encoder.WriteDouble(incoming.RobotBattery);
            }

            return encoder.Buffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte BoolToByte(bool value)
        {
            return (byte) (value ? 1 : 0);
        }

        public byte[] PackData()
        {
            if (DriverStationData.Count > 6) return null;

            List<byte> packed = new List<byte>(40*DriverStationData.Count)
            {
                (byte) DriverStationDataValues.CommunicationStructArray,
                (byte) DriverStationData.Count
            };

            foreach (var ds in DriverStationData)
            {
                packed.AddRange(PackIndividualStructure(ds));
            }

            return packed.ToArray();
        }
    }
}
