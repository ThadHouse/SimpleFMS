using SimpleFMS.Base.DriverStation.Enums;

namespace SimpleFMS.Base.DriverStation.Interfaces
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
