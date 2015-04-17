using System;
using Telligent.Evolution.Extensibility.Version1;
using TEApi = Telligent.Evolution.Extensibility.Api.Version1.PublicApi;

namespace Telligent.BigSocial.Polling.InternalApi
{
    public class PollException : Exception, ILoggableException
    {
        string ILoggableException.Category
        {
            get { return "Poll Exception"; }
        }

        public PollException() { }
        public PollException(string message) : base(message) { }
        public PollException(string message, Exception innerException) : base(message, innerException) { }
    }
}
