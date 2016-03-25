using log4net;
using System.Web.Http.ExceptionHandling;

namespace FlightNode.Common.Api.Filters
{
    /// <summary>
    /// Exception handler of last resort, for when the filters handlers are not used.
    /// </summary>
    public class LoggingExceptionHandler : ExceptionHandler, IExceptionHandler
    {
        private ILog _logger;

        public ILog Logger
        {
            get
            {
                return _logger ?? (_logger = LogManager.GetLogger(GetType().FullName));
            }
            set
            {
                _logger = value;
            }
        }


        public override void Handle(ExceptionHandlerContext context)
        {
            Logger.Error(context.Exception);

            base.Handle(context);
        }
    }
}
