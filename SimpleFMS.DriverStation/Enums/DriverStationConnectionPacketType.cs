namespace SimpleFMS.DriverStation.Enums
{
    /// <summary>
    /// The packet type send over the TCP connection to the 
    /// Driver Station
    /// </summary>
    public enum DriverStationConnectionPacketType
    {
        /// <summary>
        /// A set team packet
        /// </summary>
        SetTeam = 24,
        /// <summary>
        /// A response to the set team packet
        /// </summary>
        SendTeamResponse = 25,
        /// <summary>
        /// A ping packet from the driver station
        /// </summary>
        Ping = 28
    }
}
