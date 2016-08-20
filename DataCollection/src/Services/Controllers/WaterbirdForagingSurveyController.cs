using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Services.Models.Rookery;
using FlightNode.DataCollection.Services.Models.Survey;
using FligthNode.Common.Api.Controllers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace FlightNode.DataCollection.Services.Controllers
{
    /// <summary>
    /// API Controller for submitting Waterbird Foraging surveys.
    /// </summary>
    public class WaterbirdForagingSurveyController : LoggingController
    {

        private const string COMPLETE = "Complete";
        private const string PENDING = "Pending";
        private const string MISSING = "missing";

        private readonly IWaterbirdForagingManager _domainManager;

        /// <summary>
        /// Creates a new instance of <see cref="WaterbirdForagingSurveyController"/>.
        /// </summary>
        /// <param name="domainManager">An instance of <see cref="IWorkLogDomainManager"/></param>
        public WaterbirdForagingSurveyController(IWaterbirdForagingManager domainManager)
        {
            if (domainManager == null)
            {
                throw new ArgumentNullException(nameof(domainManager));
            }

            _domainManager = domainManager;
        }

        /// <summary>
        /// Retrieves the requested Waterbird Foraging Survey data.
        /// </summary>
        /// <param name="surveyIdentifier">
        /// Unique identifier for the survey resource to retrieve.
        /// </param>
        /// <returns>
        /// 200 with the survey data
        /// 400 if not found
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("api/v1/waterbirdforagingsurvey/{surveyIdentifier:Guid}")]
        public IHttpActionResult Get(Guid surveyIdentifier)
        {
            return WrapWithTryCatch(() =>
            {
                var result = _domainManager.FindBySurveyId(surveyIdentifier);

                if (result == null)
                {
                    return NotFound();
                }

                var model = Map(result);

                return Ok(model);
            });
        }

        /// <summary>
        /// Retrieves a a list of Waterbird Foraging information for a given user / submitter.
        /// </summary>
        /// <param name="userId">
        /// UserId of the person who submitted the survey.
        /// </param>
        /// <returns>
        /// 200 with a list of <see cref="WaterbirdForagingListItem"/> (empty list if none found)
        /// </returns>
        /// <example>
        /// </example>
        [HttpGet]
        [Authorize]
        [Route("api/WaterbirdForagingSurvey/user/{userId:int}")]
        public IHttpActionResult GetForUser(int userId)
        {
            return WrapWithTryCatch(() =>
            {
                var result = _domainManager.FindBySubmitterId(userId);

                if (result == null || !result.Any())
                {
                    return Ok(new List<WaterbirdForagingListItem>());
                }

                var models = result.Select(x =>
                {
                    return new WaterbirdForagingListItem
                    {
                        Location = x.LocationName ?? MISSING,
                        StartDate = x.StartDate.HasValue ? x.StartDate.Value.ToShortDateString() : MISSING,
                        Status = x.Finished ? "Complete" : "Pending",
                        SurveyComments = x.GeneralComments,
                        SurveyIdentifier = x.SurveyIdentifier
                    };
                });

                return Ok(models);
            });
        }

        /// <summary>
        /// Retrieves a list of all Waterbird Foraging information, including both pending and completed surveys.
        /// </summary>
        /// <returns>
        /// 200 with a list of <see cref="ForagingListItem"/>
        /// </returns>
        [HttpGet]
        [Authorize]
        public IHttpActionResult Get()
        {
            return Ok(_domainManager.GetForagingSurveyList());
        }

        /// <summary>
        /// Retrieves all completed surveys for data export.
        /// </summary>
        /// <returns>
        /// 200 with a list of <see cref="ForagingSurveyExportItem"/>
        /// </returns>
        [HttpGet]
        [Authorize]
        [Route("api/v1/waterbirdforagingsurvey/export")]
        public IHttpActionResult Export()
        {
            return Ok(_domainManager.ExportAll());
        }

        /// <summary>
        /// Creates a new waterbird foraging survey record
        /// </summary>
        /// <param name="input">An instance of <see cref="WaterbirdForagingModel"/></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public IHttpActionResult Post([FromBody]WaterbirdForagingModel input)
        {
            if (input == null)
            {
                return BadRequest("null input");
            }

            return WrapWithTryCatch(() =>
            {
                var identifier = _domainManager.NewIdentifier();

                SurveyPending entity = Map(input, identifier);

                entity = _domainManager.Create(entity);

                var result = Map(entity);

                return Created(result, identifier.ToString());
            });
        }

        private WaterbirdForagingModel Map(ISurvey input)
        {
            var entity = new WaterbirdForagingModel
            {
                AccessPointId = input.AccessPointId,
                SiteTypeId = input.AssessmentId,
                DisturbanceComments = input.DisturbanceComments,
                SurveyComments = input.GeneralComments,
                LocationId = input.LocationId,
                Temperature = input.StartTemperature,
                SurveyIdentifier = input.SurveyIdentifier,
                TideId = input.TideId,
                VantagePointId = input.VantagePointId,
                WeatherId = input.WeatherId,
                WindSpeed = input.WindSpeed,
                SurveyId = input.Id,
                Observers = input.Observers,
                WaterHeightId = input.WaterHeightId,
                StartDate = input.StartDate.HasValue ? input.StartDate.Value.ToShortDateString() : string.Empty,
                StartTime = input.StartDate.HasValue ? input.StartDate.Value.ToShortTimeString() : string.Empty,
                EndTime = input.EndDate.HasValue ? input.EndDate.Value.ToShortTimeString() : string.Empty,
            };

            foreach (var o in input.Observations)
            {
                entity.Add(new ObservationModel
                {
                    Adults = o.Bin1,
                    Juveniles = o.Bin2,
                    BirdSpeciesId = o.BirdSpeciesId,
                    FeedingId = o.FeedingSuccessRate,
                    HabitatId = o.HabitatTypeId,
                    PrimaryActivityId = o.PrimaryActivityId,
                    SecondaryActivityId = o.SecondaryActivityId,
                    ObservationId = o.Id
                });
            }

            foreach (var d in input.Disturbances)
            {
                entity.Add(new DisturbanceModel
                {
                    DisturbanceTypeId = d.DisturbanceTypeId,
                    DurationMinutes = d.DurationMinutes,
                    Quantity = d.Quantity,
                    Behavior = d.Result,
                    DisturbanceId = d.Id
                });
            }

            return entity;
        }

        /// <summary>
        /// Updates an existing new waterbird foraging survey record
        /// </summary>
        /// <param name="surveyIdentifier"></param>
        /// <param name="input">An instance of <see cref="WaterbirdForagingModel"/></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/waterbirdforagingsurvey/{surveyIdentifier:Guid}")]
        [Authorize]
        public IHttpActionResult Put(Guid surveyIdentifier, [FromBody]WaterbirdForagingModel input)
        {
            if (input == null)
            {
                return BadRequest("null input");
            }

            if (surveyIdentifier == Guid.Empty)
            {
                return BadRequest("Invalid Survey Identifier");
            }

            return WrapWithTryCatch(() =>
            {
                SurveyPending entity = Map(input, surveyIdentifier);

                WaterbirdForagingModel result;

                if (input.Finished)
                {
                    result = Map(_domainManager.Finish(entity));
                }
                else
                {
                    result = Map(_domainManager.Update(entity));
                }


                return Ok(result);
            });
        }

        private SurveyPending Map(WaterbirdForagingModel input, Guid identifier)
        {

            var entity = new SurveyPending
            {
                AccessPointId = input.AccessPointId,
                AssessmentId = input.SiteTypeId,
                DisturbanceComments = input.DisturbanceComments,
                EndTemperature = null,
                GeneralComments = input.SurveyComments,
                LocationId = input.LocationId,
                StartTemperature = input.Temperature,
                SurveyIdentifier = identifier,
                TideId = input.TideId,
                SurveyTypeId = SurveyType.Foraging,
                VantagePointId = input.VantagePointId,
                WeatherId = input.WeatherId,
                WindSpeed = input.WindSpeed,
                SubmittedBy = this.LookupUserId(),
                Observers = input.Observers,
                Id = input.SurveyId,
                WaterHeightId = input.WaterHeightId,
                StartDate = ParseDateTime(input.StartDate, input.StartTime),
                EndDate = ParseDateTime(input.StartDate, input.EndTime),
            };


            foreach (var o in input.Observations)
            {
                entity.Add(new Observation
                {
                    Bin1 = o.Adults,
                    Bin2 = o.Juveniles,
                    BirdSpeciesId = o.BirdSpeciesId,
                    FeedingSuccessRate = o.FeedingId,
                    HabitatTypeId = o.HabitatId,
                    PrimaryActivityId = o.PrimaryActivityId,
                    SecondaryActivityId = o.SecondaryActivityId,
                    SurveyIdentifier = identifier,
                    Id = o.ObservationId
                });
            }

            foreach (var d in input.Disturbances)
            {
                entity.Add(new Disturbance
                {
                    DisturbanceTypeId = d.DisturbanceTypeId,
                    DurationMinutes = d.DurationMinutes,
                    Quantity = d.Quantity,
                    Result = d.Behavior,
                    SurveyIdentifier = identifier,
                    Id = d.DisturbanceId
                });
            }

            return entity;
        }

        private DateTime? ParseDateTime(string date, string time)
        {
            date = date ?? string.Empty;
            time = time ?? string.Empty;

            var dateOnly = date.Contains("T") ? date.Split('T')[0] : date;
            var timeOnly = time.Contains("T") ? time.Split('T')[1] : time;

            string combined;
            if (timeOnly.Contains("M"))
            {
                combined = dateOnly + " " + timeOnly;
            }
            else
            {
                combined = dateOnly + "T" + timeOnly;
            }

            DateTime dateTime;
            if (DateTime.TryParse(combined, out dateTime))
            {
                return dateTime;
            }

            return null;
        }

        private int RetrieveCurrentUserId()
        {
            return User.Identity.GetUserId<int>();
        }
    }
}
