using System.Web.Http.Filters;


namespace FlightNode.Common.Api.Filters
{
    public class UnhandledExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            // do something with context.Exception, e.g. log it
        }
    }
}
