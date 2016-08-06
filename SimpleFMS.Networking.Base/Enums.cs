namespace SimpleFMS.Networking.Base
{
    public enum CustomNetworkTableType
    {
        DriverStationReport = 1,
        DriverStationConfiguration,
        DriverStationUpdateBypass,
        DriverStationUpdateEStop,
        DriverStationGlobalEStop,
        
        MatchTimingReport,
        MatchTimingStartMatch,
        MatchTimingStopPeriod,
        MatchTimingStartAutonomous,
        MatchTimingStartTeleoperated,
        MatchTimingSetPeriodTimes
    }
}
