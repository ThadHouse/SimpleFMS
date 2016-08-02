using SimpleFMS.DriverStation.Base.Enums;

namespace SimpleFMS.DriverStation.Base.Interfaces
{
    /// <summary>
    /// Interface that parses a packet and store it internally
    /// </summary>
    public interface IReceiveParser
    {
        /// <summary>
        /// Parse the incoming packet
        /// </summary>
        /// <param name="data">The data to pass</param>
        /// <returns>The status of the parsing</returns>
        ReceiveParseStatus ParseData(byte[] data);
    }
}
