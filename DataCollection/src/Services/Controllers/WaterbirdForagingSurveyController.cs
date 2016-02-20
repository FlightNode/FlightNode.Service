using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Services.Models.Rookery;
using FligthNode.Common.Api.Controllers;
using Microsoft.AspNet.Identity;
using System;
using System.Web.Http;

namespace FlightNode.DataCollection.Services.Controllers
{
    /// <summary>
    /// API Controller for submitting Waterbird Foraging surveys.
    /// </summary>
    public class WaterbirdForagingSurveyController : LoggingController
    {

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
        /// Creates a new waterbird foraging survey record
        /// </summary>
        /// <param name="input">An instance of <see cref="WaterbirdForagingModel"/></param>
        /// <returns></returns>
        [HttpPost]
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

                input.SurveyId = _domainManager.Create(entity);
                input.SurveyIdentifier = identifier;

                var output = Created(input, identifier.ToString());

                return output;
            });
        }

        /// <summary>
        /// Updates an existing new waterbird foraging survey record
        /// </summary>
        /// <param name="input">An instance of <see cref="WaterbirdForagingModel"/></param>
        /// <returns></returns>
        [HttpPut]
        public IHttpActionResult Put([FromBody]WaterbirdForagingModel input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.SurveyIdentifier == null ||
                input.SurveyIdentifier == Guid.Empty)
            {
                return BadRequest("Invalid Survey Identifier");
            }

            return WrapWithTryCatch(() =>
            {
                SurveyPending entity = Map(input, input.SurveyIdentifier.Value);

                _domainManager.Update(entity, input.Step);

                return NoContent();
            });
        }

        private SurveyPending Map(WaterbirdForagingModel input, Guid identifier)
        {
            var entity = new SurveyPending
            {
                AccessPointId = input.AccessPointInfoId,
                AssessmentId = input.SiteTypeId,
                DisturbanceComments = input.DisturbanceComments,
                EndDate = input.EndDate,
                EndTemperature = null,
                GeneralComments = input.SurveyComments,
                LocationId = input.LocationId,
                StartDate = input.StartDate,
                StartTemperature = input.Temperature,
                SurveyIdentifier = identifier,
                TideId = input.TideInfoId,
                SurveyTypeId = SurveyType.TERN_FORAGING,
                TimeOfLowTide = input.TimeOfLowTide,
                VantagePointId = input.VantagePointInfoId,
                WeatherId = input.WeatherInfoId,
                WindSpeedId = input.WindSpeed,
                SubmittedBy = RetrieveCurrentUserId()
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
                    SurveyIdentifier = identifier
                });
            }

            foreach (var u in input.Observers)
            {
                entity.Add(u);
            }

            return entity;
        }

        private int RetrieveCurrentUserId()
        {
            return User.Identity.GetUserId<int>();
        }
    }
}
