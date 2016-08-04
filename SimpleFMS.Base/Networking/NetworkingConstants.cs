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
        }
    }
}
