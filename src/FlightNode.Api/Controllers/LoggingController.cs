using System;
using System.Web.Http;
using FlightNode.Common.Exceptions;
using System.Web.Http.ModelBinding;
using System.Linq;
using log4net;

namespace FligthNode.Common.Api.Controllers
{
    public abstract class LoggingController : ApiController
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


        protected IHttpActionResult WrapWithTryCatch(Func<IHttpActionResult> func)
        {
            try
            {
                return func();
            }
            catch (UserException uex)
            {
                return Handle(uex);
            }
            catch (DomainValidationException dex)
            {
                return Handle(dex);
            }
            catch (Exception ex)
            {
                return Handle(ex);
            }
        }

        protected IHttpActionResult Handle(UserException ex)
        {
            // No *error* logging necessary when it was a user-induced error
            // TODO: however, it might be a good idea to write this out to a low
            // level log (lower than error or warning), so that we can turn on
            // the logging for it when needed. But in that case, may want more 
            // information about the issue, like which function was being hit.
            // Will need to think about it more.

            return BadRequest(ex.Message);
        }

        protected IHttpActionResult Handle(DomainValidationException ex)
        {
            // No logging necessary when it was a user-induced error
            
            return BadRequest(ConvertToModelStateErrors(ex));
        }

        private static ModelStateDictionary ConvertToModelStateErrors(DomainValidationException ex)
        {
            var modelState = new ModelStateDictionary();

            ex.ValidationResults.ToList().ForEach(x =>
            {
                x.MemberNames.ToList().ForEach(y =>
                {
                    modelState.AddModelError(y, x.ErrorMessage);
                });
            });
            return modelState;
        }

        protected IHttpActionResult Handle(Exception ex)
        {
            Logger.Error(ex);

            return InternalServerError();
        }


        protected internal virtual IHttpActionResult NoContent()
        {
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }
    }
}
