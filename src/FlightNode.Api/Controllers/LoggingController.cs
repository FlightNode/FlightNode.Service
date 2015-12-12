using NLog;
using System;
using System.Collections.Concurrent;
using System.Web.Http;
using FlightNode.Common.Exceptions;
using System.Web.Http.ModelBinding;
using System.Linq;

namespace FligthNode.Common.Api.Controllers
{
    public abstract class LoggingController : ApiController
    {
        private ILogger _logger;

        public ILogger Logger
        {
            get
            {
                if (_logger ==null)
                {
                    _logger = LogManager.GetLogger(GetType().FullName);
                }
                return _logger;
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
            // No logging necessary when it was a user-induced error

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
