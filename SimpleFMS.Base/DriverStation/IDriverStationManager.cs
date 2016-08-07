using System;
using System.Collections.Generic;
using SimpleFMS.Base.Enums;

namespace SimpleFMS.Base.DriverStation
{
    /// <summary>
    /// An interface for dealing with all driver stations connected
    /// to an FMS.
    /// </summary>
    public interface IDriverStationManager : IDisposable
    {
        /// <summary>
        /// Occurs when any driver stations status is updated
        /// </summary>
        event Action<IReadOnlyDictionary<AllianceStation, IDriverStationReport>> OnDriverStationStatusChanged;

        /// <summary>
        /// Gets a dictionary containing all drivers stations and their current status
        /// </summary>
        IReadOnlyDictionary<AllianceStation, IDriverStationReport> DriverStations { get; }

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

        /// <summary>
        /// Sets the time remaining in the current match period
        /// </summary>
        /// <param name="remainingTime">The time in seconds</param>
        void SetRemainingMatchTime(int remainingTime);

        /// <summary>
        /// Sets a specific alliance station to be bypassed
        /// </summary>
        /// <param name="station">The alliance station to bypass</param>
        /// <param name="bypassed">True to bypass, false otherwise</param>
        void SetBypass(AllianceStation station, bool bypassed);

        /// <summary>
        /// Sets a specific alliance station to be eStopped
        /// </summary>
        /// <param name="station">The alliance station to bypass</param>
        /// <param name="eStopped">True to eStop, false otherwise</param>
        void SetEStop(AllianceStation station, bool eStopped);
    }
}
