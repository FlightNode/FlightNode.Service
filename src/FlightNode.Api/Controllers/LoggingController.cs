using NLog;
using System;
using System.Collections.Concurrent;
using System.Web.Http;
using FlightNode.Common.Exceptions;

namespace FligthNode.Common.Api.Controllers
{
    public abstract class LoggingController : ApiController
    {
        // Loggers do not maintain state, and therefore it should be safe to keep one 
        // logger for each controller in memory rather than rebuild one with every
        // use of the logger. But, we need to ensure thread safety, and thus using
        // the thread-safe dictionary.
        private static ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

        protected virtual ILogger Logger
        {
            get
            {
                var fullName = GetType().FullName;
                if (_loggers.ContainsKey(fullName))
                {
                    return _loggers[fullName];
                }
                else
                {
                    var logger = LogManager.GetLogger(fullName);
                    _loggers[fullName] = logger;
                    return logger;
                }
            }
        }

        protected IHttpActionResult Handle(UserException ex)
        {
            return BadRequest(ex.Message);
        }

        protected IHttpActionResult Handle(Exception ex)
        {
            Logger.Error(ex);

            return InternalServerError();
        }
    }
}
