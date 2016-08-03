using System.Collections.Generic;
using SimpleFMS.Base.DriverStation.Enums;
using SimpleFMS.Base.Tuples;

namespace SimpleFMS.Base.DriverStation.Interfaces
{
    /// <summary>
    /// An interface for dealing with all driver stations connected
    /// to an FMS.
    /// </summary>
    public interface IDriverStationManager
    {
        IReadOnlyList<IDriverStationConfiguration> ConnectedDriverStations { get; }
        IReadOnlyDictionary<ValueTuple<AllianceStationSide, AllianceStationNumber>, int> RequestedDriverStations
        { get; }

        /// <summary>
        /// Initializes a new match to be played
        /// </summary>
        /// <param name="driverStationConfigurations">A list of driver station configurations</param>
        /// <param name="matchNumber">The current match number</param>
        /// <param name="matchType">The current match type</param>
        /// <returns>True if matches were added successfully, otherwise false</returns>
        bool InitializeMatch(IReadOnlyList<IDriverStationConfiguration> driverStationConfigurations, int matchNumber,
            MatchType matchType);

        /// <summary>
        /// Starts a match period
        /// </summary>
        /// <param name="auto">True if autonomous, false if teleop</param>
        void StartMatchPertiod(bool auto);

        /// <summary>
        /// Stops a match period
        /// </summary>
        void StopMatchPeriod();

        void SetRemainingMatchTime(int remainingTime);

        /// <summary>
        /// Sets a specific alliance station to be bypassed
        /// </summary>
        /// <param name="alliance">The alliance side to bypass</param>
        /// <param name="station">The alliance station to bypass</param>
        /// <param name="bypassed">True to bypass, false otherwise</param>
        void SetBypass(AllianceStationSide alliance, AllianceStationNumber station, bool bypassed);

        /// <summary>
        /// Sets a specific alliance station to be eStopped
        /// </summary>
        /// <param name="alliance">The alliance side to bypass</param>
        /// <param name="station">The alliance station to bypass</param>
        /// <param name="eStopped">True to eStop, false otherwise</param>
        void SetEStop(AllianceStationSide alliance, AllianceStationNumber station, bool eStopped);
    }
}
