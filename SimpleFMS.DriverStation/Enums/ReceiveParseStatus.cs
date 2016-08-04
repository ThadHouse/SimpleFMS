namespace SimpleFMS.DriverStation.Enums
{
    /// <summary>
    /// Represents the status of a Receive Parse
    /// </summary>
    public enum ReceiveParseStatus
    {
        /// <summary>
        /// The packet is valid, and a type we track
        /// </summary>
        TrackedPacketValid,
        /// <summary>
        /// The packet is invalid, but is a type we track
        /// </summary>
        TrackedPacketInvalid,
        /// <summary>
        /// The packet is valid, but is a type we don't track
        /// </summary>
        UntrackedPacket,
        /// <summary>
        /// The packet is invalid, with no way to get its type
        /// </summary>
        InvalidPacket
    }
}
