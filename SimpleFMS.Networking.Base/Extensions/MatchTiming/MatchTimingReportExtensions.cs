using System;
using System.Collections.Generic;
using System.Net;
using SimpleFMS.Base.Enums;
using SimpleFMS.Base.MatchTiming;

namespace SimpleFMS.Networking.Base.Extensions.MatchTiming
{
    public static class MatchTimingReportExtensions
    {
        private static void AddTimeSpanToReport(this TimeSpan span, List<byte> addTo)
        {
            byte[] remaining =
                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(BitConverter.DoubleToInt64Bits(span.TotalSeconds)));
            addTo.AddRange(remaining);
        }

        private static TimeSpan GetTimeSpanFromReport(this byte[] data, ref int startIndex)
        {
            double seconds =
                BitConverter.Int64BitsToDouble(IPAddress.NetworkToHostOrder(BitConverter.ToInt64(data, startIndex)));
            startIndex += 8;
            return TimeSpan.FromSeconds(seconds);
        }

        public static byte[] PackMatchTimingReport(this IMatchTimingReport report)
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

        public static IMatchTimingReport GetMatchTimingReport(this byte[] bytes)
        {
            if (bytes.Length < 34)
                throw new ArgumentOutOfRangeException(nameof(bytes), "Bytes must have a length of at least 34 bytes");

            if (bytes[0] != (byte) CustomNetworkTableType.MatchTimingReport)
                return null;

            MatchTimingReport report = new MatchTimingReport
            {
                MatchState = (MatchState) bytes[1]
            };
            int index = 2;
            report.RemainingPeriodTime = GetTimeSpanFromReport(bytes, ref index);
            report.AutonomousTime = GetTimeSpanFromReport(bytes, ref index);
            report.DelayTime = GetTimeSpanFromReport(bytes, ref index);
            report.TeleoperatedTime = GetTimeSpanFromReport(bytes, ref index);
            return report;
        }
    }
}
