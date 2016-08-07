using System.Collections.Generic;
using SimpleFMS.Base.MatchTiming;

namespace SimpleFMS.Networking.Base.Extensions.MatchTiming
{
    public static class MatchTimingMatchStateExtensions
    {
        // Start Match
        public static byte[] PackStartMatch()
        {
            byte[] data = new byte[1];
            data[0] = (byte) CustomNetworkTableType.MatchTimingStartMatch;
            return data;
        }

        public static bool UnpackStartMatch(this byte[] data)
        {
            if (data.Length < 1) return false;
            return data[0] == (byte) CustomNetworkTableType.MatchTimingStartMatch;
        }

        public static byte[] PackStartMatchResponse(bool success)
        {
            byte[] data = new byte[2];
            data[0] = (byte)CustomNetworkTableType.MatchTimingStartMatch;
            data[1] = (byte) (success ? 1 : 0);
            return data;
        }

        public static bool UnpackStartMatchResponse(this byte[] data)
        {
            if (data.Length < 2) return false;
            if (data[0] != (byte) CustomNetworkTableType.MatchTimingStartMatch) return false;
            return data[1] != 0;
        }

        // Start Autonomous
        public static byte[] PackStartAutonomous()
        {
            byte[] data = new byte[1];
            data[0] = (byte)CustomNetworkTableType.MatchTimingStartAutonomous;
            return data;
        }

        public static bool UnpackStartAutonomous(this byte[] data)
        {
            if (data.Length < 1) return false;
            return data[0] == (byte)CustomNetworkTableType.MatchTimingStartAutonomous;
        }

        public static byte[] PackStartAutonomousResponse(bool success)
        {
            byte[] data = new byte[2];
            data[0] = (byte)CustomNetworkTableType.MatchTimingStartAutonomous;
            data[1] = (byte)(success ? 1 : 0);
            return data;
        }

        public static bool UnpackStartAutonomousResponse(this byte[] data)
        {
            if (data.Length < 2) return false;
            if (data[0] != (byte)CustomNetworkTableType.MatchTimingStartAutonomous) return false;
            return data[1] != 0;
        }

        // Start Teleoperated
        public static byte[] PackStartTeleoperated()
        {
            byte[] data = new byte[1];
            data[0] = (byte)CustomNetworkTableType.MatchTimingStartTeleoperated;
            return data;
        }

        public static bool UnpackStartTeleoperated(this byte[] data)
        {
            if (data.Length < 1) return false;
            return data[0] == (byte)CustomNetworkTableType.MatchTimingStartTeleoperated;
        }

        public static byte[] PackStartTeleoperatedResponse(bool success)
        {
            byte[] data = new byte[2];
            data[0] = (byte)CustomNetworkTableType.MatchTimingStartTeleoperated;
            data[1] = (byte)(success ? 1 : 0);
            return data;
        }

        public static bool UnpackStartTeleoperatedResponse(this byte[] data)
        {
            if (data.Length < 2) return false;
            if (data[0] != (byte)CustomNetworkTableType.MatchTimingStartTeleoperated) return false;
            return data[1] != 0;
        }

        // Stop Period
        public static byte[] PackStopPeriod()
        {
            byte[] data = new byte[1];
            data[0] = (byte)CustomNetworkTableType.MatchTimingStopPeriod;
            return data;
        }

        public static bool UnpackStopPeriod(this byte[] data)
        {
            if (data.Length < 1) return false;
            return data[0] == (byte)CustomNetworkTableType.MatchTimingStopPeriod;
        }

        public static byte[] PackStopPeriodResponse(bool success)
        {
            byte[] data = new byte[2];
            data[0] = (byte)CustomNetworkTableType.MatchTimingStopPeriod;
            data[1] = (byte)(success ? 1 : 0);
            return data;
        }

        public static bool UnpackStopPeriodResponse(this byte[] data)
        {
            if (data.Length < 2) return false;
            if (data[0] != (byte)CustomNetworkTableType.MatchTimingStopPeriod) return false;
            return data[1] != 0;
        }

        // Set Match Times
        public static byte[] PackMatchTimes(this IMatchTimeReport times)
        {
            List<byte> data = new List<byte>();
            data.Add((byte)CustomNetworkTableType.MatchTimingSetPeriodTimes);
            times.AutonomousTime.AddTimeSpanToReport(data);
            times.DelayTime.AddTimeSpanToReport(data);
            times.TeleoperatedTime.AddTimeSpanToReport(data);
            return data.ToArray();
        }

        public static IMatchTimeReport UnpackMatchTimes(this byte[] data)
        {
            if (data.Length < 25) return null;
            if (data[0] != (byte) CustomNetworkTableType.MatchTimingSetPeriodTimes)
                return null;
            int index = 1;
            var autonomousTime = data.GetTimeSpanFromReport(ref index);
            var delayTime = data.GetTimeSpanFromReport(ref index);
            var teleoperatedTime = data.GetTimeSpanFromReport(ref index);
            MatchTimeReport times = new MatchTimeReport(teleoperatedTime, delayTime, autonomousTime);
            
            return times;
        }

        public static byte[] PackMatchTimesResponse(bool success)
        {
            byte[] data = new byte[2];
            data[0] = (byte)CustomNetworkTableType.MatchTimingSetPeriodTimes;
            data[1] = (byte) (success ? 1 : 0);
            return data;
        }

        public static bool UnpackMatchTimesResponse(this byte[] data)
        {
            if (data.Length < 2) return false;
            if (data[0] != (byte)CustomNetworkTableType.MatchTimingSetPeriodTimes) return false;
            return data[1] != 0;
        }
    }
}
