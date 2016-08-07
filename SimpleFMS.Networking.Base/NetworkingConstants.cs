namespace SimpleFMS.Base.Networking
{
    public static class NetworkingConstants
    {
        public const int NetworkTablesPort = 1760;
        public const int NetworkTablesVersion = 0x0300;
        public const string FmsServerRemoteName = "SimpleFMS Server";
        public const string PersistentFilename = "SimpleFMSPersistence.txt";
        public const string ListenAddress = "";
        public const string FmsIpAddress = "10.0.100.5";

        public const string RootTableName = "FMS";

        public static class DriverStationConstants
        {
            public const string DriverStationTableName = "DriverStation";
            public const string DriverStationReportKey = "DriverStationReports";
            public const string DriverStationSetConfigurationRpcKey = "RpcDriverStationSetConfiguration";
            public const int DriverStationSetConfigurationRpcVersion = 1;
            public const string DriverStationUpdateBypassRpcKey = "RpcDriverStationUpdateBypass";
            public const int DriverStationUpdateBypassRpcVersion = 1;
            public const string DriverStationUpdateEStopRpcKey = "RpcDriverStationUpdateEStop";
            public const int DriverStationUpdateEStopRpcVersion = 1;
            public const string DriverStationGlobalEStopRpcKey = "RpcDriverStationGlobalEStop";
            public const int DriverStationGlobalEStopVersion = 1;
        }

        public static class MatchTimingConstants
        {
            public const string MatchTimingTableName = "MatchTiming";
            public const string StartMatchRpcKey = "RpcStartMatch";
            public const string StopPeriodRpcKey = "RpcStopPeriod";
            public const string StartAutonomousRpcKey = "RpcStartAutonomous";
            public const string StartTeleoperatedRpcKey = "RpcStartTeleoperated";
            public const string SetMatchPeriodTimeRpcKey = "RpcSetMatchPeriodTime";
            public const string MatchStatusReportKey = "MatchStatusReports";

            public const int StartMatchRpcVersion = 1;
            public const int StopPeriodRpcVersion = 1;
            public const int StartAutonomousRpcVersion = 1;
            public const int StartTeleoperatedRpcVersion = 1;
            public const int SetMatchPeriodTimeRpcVersion = 1;
            public const int MatchStatusReportVersion = 1;
        }
    }
}
