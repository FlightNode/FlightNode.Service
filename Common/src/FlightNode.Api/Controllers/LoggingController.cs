using System;
using System.Web.Http;
using FlightNode.Common.Exceptions;
using System.Web.Http.ModelBinding;
using System.Linq;
using log4net;
using Flurl;

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
            Logger.Debug(ex);

            return BadRequest(ex.Message);
        }

        protected IHttpActionResult Handle(DomainValidationException ex)
        {
            Logger.Debug(ex);

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

        protected internal virtual IHttpActionResult MethodNotAllowed()
        {
            return StatusCode(System.Net.HttpStatusCode.MethodNotAllowed);
        }



        protected IHttpActionResult Created<TModel>(TModel input, string id)
        {
            var locationHeader = this.Request
                .RequestUri
                .ToString()
                .AppendPathSegment(id);

            return Created(locationHeader, input);
        }
    }
}
