using System;
using System.Collections.Generic;
using System.Net;
using SimpleFMS.Base.Enums;
using SimpleFMS.Base.MatchTiming;

namespace SimpleFMS.Networking.Base.Extensions.MatchTiming
{
    public static class MatchTimingReportExtensions
    {
        internal static void AddTimeSpanToReport(this TimeSpan span, List<byte> addTo)
        {
            byte[] remaining =
                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(BitConverter.DoubleToInt64Bits(span.TotalSeconds)));
            addTo.AddRange(remaining);
        }

        internal static TimeSpan GetTimeSpanFromReport(this byte[] data, ref int startIndex)
        {
            double seconds =
                BitConverter.Int64BitsToDouble(IPAddress.NetworkToHostOrder(BitConverter.ToInt64(data, startIndex)));
            startIndex += 8;
            return TimeSpan.FromSeconds(seconds);
        }

        public static byte[] PackMatchTimingReport(this IMatchStateReport report)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)CustomNetworkTableType.MatchTimingReport);
            data.Add((byte)report.MatchState);
            report.RemainingPeriodTime.AddTimeSpanToReport(data);
            report.AutonomousTime.AddTimeSpanToReport(data);
            report.DelayTime.AddTimeSpanToReport(data);
            report.TeleoperatedTime.AddTimeSpanToReport(data);
            return data.ToArray();
        }

        public static IMatchStateReport GetMatchTimingReport(this byte[] bytes)
        {
            if (bytes.Length < 34)
                return null;

            if (bytes[0] != (byte) CustomNetworkTableType.MatchTimingReport)
                return null;
            int index = 2;
            var RemainingPeriodTime = GetTimeSpanFromReport(bytes, ref index);
            var AutonomousTime = GetTimeSpanFromReport(bytes, ref index);
            var DelayTime = GetTimeSpanFromReport(bytes, ref index);
            var TeleoperatedTime = GetTimeSpanFromReport(bytes, ref index);
            var MatchState = (MatchState) bytes[1];
            MatchStateReport report = new MatchStateReport(MatchState, RemainingPeriodTime, TeleoperatedTime, DelayTime,
                AutonomousTime);
            
            return report;
        }
    }
}
