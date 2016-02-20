using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Services.Models.WorkLog;
using FligthNode.Common.Api.Controllers;
using Flurl;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Http;

namespace FlightNode.DataCollection.Domain.Services.Controllers
{
    public class WorkLogsController : LoggingController
    {

        private readonly IWorkLogDomainManager _domainManager;

        /// <summary>
        /// Creates a new instance of <see cref="LocationsController"/>.
        /// </summary>
        /// <param name="domainManager">An instance of <see cref="IWorkLogDomainManager"/></param>
        public WorkLogsController(IWorkLogDomainManager domainManager)
        {
            if (domainManager == null)
            {
                throw new ArgumentNullException(nameof(domainManager));
            }

            _domainManager = domainManager;
        }

        /// <summary>
        /// Retrieves all Work Type representations.
        /// </summary>
        /// <returns>Action result containing an enumeration of work types</returns>
        /// <example>
        /// GET: /api/v1/worklogs
        /// </example>
        [Authorize(Roles = "Administrator,Coordinator,Lead")]
        public IHttpActionResult Get()
        {
            return WrapWithTryCatch(() =>
            {
                var locations = _domainManager.FindAll();

                var models = locations.Select(x => new WorkLogModel
                {
                    LocationId = x.LocationId,
                    TravelTimeHours = x.TravelTimeHours,
                    UserId = x.UserId,
                    WorkDate = x.WorkDate,
                    WorkHours = x.WorkHours,
                    WorkTypeId = x.WorkTypeId,
                    Id = x.Id
                });

                return Ok(models);
            });
        }

        /// <summary>
        /// Retrieves a specific work type representation.
        /// </summary>
        /// <param name="id">Unique identifier for the work type resource</param>
        /// <returns>Action result containing a representation of the requested work types</returns>
        /// <example>
        /// GET: /api/v1/worklogs/123
        /// </example>
        [Authorize]
        public IHttpActionResult Get(int id)
        {
            return WrapWithTryCatch(() =>
            {
                var x = _domainManager.FindById(id);

                WorkLogModel model = Map(x);

                return Ok(model);
            });
        }

        private static WorkLogModel Map(WorkLog x)
        {
            return new WorkLogModel
            {
                LocationId = x.LocationId,
                TravelTimeHours = x.TravelTimeHours,
                UserId = x.UserId,
                WorkDate = x.WorkDate,
                WorkHours = x.WorkHours,
                WorkTypeId = x.WorkTypeId,
                Id = x.Id
            };
        }


        /// <summary>
        /// Retrieves all of the work log entries only for the logged-in user.
        /// </summary>
        /// <returns>Action result containing information about the user's work log entries</returns>
        /// <example>
        /// GET: /api/v1/worklogs/my
        /// </example>
        [Authorize]
        [Route("api/v1/worklogs/my")]
        [HttpGet]
        public IHttpActionResult GetMyLogs()
        {
            return WrapWithTryCatch(() =>
            {
                int userId = RetrieveCurrentUserId();

                var data = _domainManager.GetForUser(userId);

                var models = data.Select(MyWorkLogModel.CreateFrom).ToList();

                return Ok(models);
            });
        }

        private int RetrieveCurrentUserId()
        {
            return User.Identity.GetUserId<int>();
        }

        /// <summary>
        /// Retrieves all work log entries for all users.
        /// </summary>
        /// <returns>Action result containing information about all work log entries</returns>
        /// <example>
        /// GET: /api/v1/worklogs/my
        /// </example>
        [Authorize(Roles = "Administrator,Coordinator")]
        [Route("api/v1/worklogs/export")]
        public IHttpActionResult GetExport()
        {
            return WrapWithTryCatch(() =>
            {
                var data = _domainManager.GetReport();

                var models = data.Select(WorkLogReportModel.CreateFrom).ToList();

                return Ok(models);
            });
        }

        /// <summary>
        /// Creates a new work type resource.
        /// </summary>
        /// <param name="input">Complete parameters of the work type resource</param>
        /// <returns>Action result containing the new resource's permanent URL</returns>
        /// <example>
        /// POST: /api/v1/worklogs
        /// {
        ///   "locationdId": 1
        ///   "travelTimeHours": 1.53,
        ///   "userId": 43,
        ///   "workDate": "2015-12-07 13:43", 
        ///   "WorkHours": 4.2,
        ///   "workTypeId": 3,
        /// }
        /// </example>
        /// <remarks>
        /// The work date does not need to be in UTC. It is assumed to be local to the 
        /// location. In any statistical calculations, it is the relative time of day
        /// that matters, not the absolute (UTC) time.
        /// </remarks>
        [Authorize]
        [HttpPost]
        public IHttpActionResult Post([FromBody]WorkLogModel input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }

            return WrapWithTryCatch(() =>
            {
                var WorkLog = new WorkLog
                {
                    LocationId = input.LocationId,
                    TravelTimeHours = input.TravelTimeHours,
                    UserId = input.UserId,
                    WorkDate = input.WorkDate,
                    WorkHours = input.WorkHours,
                    WorkTypeId = input.WorkTypeId,
                };

                WorkLog = _domainManager.Create(WorkLog);

                var locationHeader = this.Request
                    .RequestUri
                    .ToString()
                    .AppendPathSegment(WorkLog.Id.ToString());

                return Created(locationHeader, WorkLog);
            });
        }

        /// <summary>
        /// Updates an existing work type resource.
        /// </summary>
        /// <param name="input">Complete parameters of the work type resource</param>
        /// <returns>Action result with status code 204 "no content"</returns>
        /// <example>
        /// PUT: /api/v1/worklogs/123
        /// {
        ///   "locationdId": 1
        ///   "id": 3,
        ///   "travelTimeHours": 1.53,
        ///   "userId": 43,
        ///   "workDate": "2015-12-07 13:43", 
        ///   "WorkHours": 4.2,
        ///   "workTypeId": 3,
        ///   "id": 46646
        /// }
        /// </example>
        /// <remarks>
        /// The work date does not need to be in UTC. It is assumed to be local to the 
        /// location. In any statistical calculations, it is the relative time of day
        /// that matters, not the absolute (UTC) time.
        /// </remarks>
        [Authorize]
        [HttpPut]
        public IHttpActionResult Put([FromBody]WorkLogModel input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }

            return WrapWithTryCatch(() =>
            {
                var location = new WorkLog
                {
                    LocationId = input.LocationId,
                    TravelTimeHours = input.TravelTimeHours,
                    UserId = input.UserId,
                    WorkDate = input.WorkDate,
                    WorkHours = input.WorkHours,
                    WorkTypeId = input.WorkTypeId,
                    Id = input.Id
                };

                _domainManager.Update(location);

                return NoContent();
            });
        }

    }
}
