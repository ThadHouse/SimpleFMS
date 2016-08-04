namespace SimpleFMS.Base.Networking
{
    public static class NetworkingConstants
    {
        public const int NetworkTablesPort = 1755;
        public const uint NetworkTablesVersion = 0x0300;
        public const string ServerRemoteName = "SimpleFMS Server";

        public const string RootTableName = "FMS";

        public static class DriverStationConstants
        {
            public const string DriverStationTableName = "DriverStation";
            public const string DriverStationReportKey = "DriverStationReports";
            public const string DriverStationTeamUpdateRpcKey = "RpcDriverStationTeamUpdate";
            public const string DriverStationUpdateBypassKey = "RpcDriverStationUpdateBypass";
            public const string DriverStationUpdateEStopKey = "RpcDriverStationUpdateEStop";
        }

        public static class MatchTimingConstatns
        {
            public const string MatchTimingTableName = "MatchTiming";
            public const string StartMatchRpcKey = "RpcStartMatch";
            public const string StopPeriodRpcKey = "RpcStopPeriod";
            public const string StartAutonomousRpcKey = "RpcStartAutonomous";
            public const string StartTeleoperatedRpcKey = "RpcStartTeleoperated";
            public const string SetMatchPeriodTimeRpcKey = "RpcSetMatchPeriodTime";
            public const string MatchTimingReportKey = "MatchTimingReports";
        }
    }
}
