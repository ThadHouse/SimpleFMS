using System;

namespace SimpleFMS.Base.Exceptions
{
    /// <summary>
    /// Exception stating an operation cannot be completed successfully because the match is running
    /// </summary>
    public class MatchEnabledException : InvalidOperationException
    {
        /// <summary>
        /// Creates a new MatchEnabledException with a blank message
        /// </summary>
        public MatchEnabledException()
            : this("The operation cannot be completed because the match is currently enabled.") { }

        /// <summary>
        /// Creates a new MatchEnabledException with the specified message
        /// </summary>
        /// <param name="message">The message for the exception</param>
        public MatchEnabledException(string message) : base(message) { }
    }
}
