using System;

namespace SimpleFMS.Base.Exceptions
{
    public class MatchEnabledException : InvalidOperationException
    {
        public MatchEnabledException()
            : this("The operation cannot be completed because the match is currently enabled.") { }

        public MatchEnabledException(string message) : base(message) { }
    }
}
