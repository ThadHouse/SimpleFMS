namespace SimpleFMS.Base.DriverStation.Interfaces
{
    /// <summary>
    /// An interface to pack internal variable into a byte array
    /// to send over a socket or over NetworkTables as a raw type
    /// </summary>
    public interface ISendPacker
    {
        /// <summary>
        /// Packs the internal data into a byte array
        /// </summary>
        /// <returns>The packed data</returns>
        byte[] PackData();
    }
}
