using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Domain.Services.Models;
using FligthNode.Common.Api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace FlightNode.DataCollection.Domain.Services.Controllers
{
    public class LocationController : LoggingController
    {

        private readonly ILocationDomainManager _domainManager;

        /// <summary>
        /// Creates a new instance of <see cref="LocationController"/>.
        /// </summary>
        /// <param name="domainManager">An instance of <see cref="ILocationDomainManager"/></param>
        public LocationController(ILocationDomainManager domainManager)
        {
            if (domainManager == null)
            {
                throw new ArgumentNullException("domainManager");
            }

            _domainManager = domainManager;
        }

        /// <summary>
        /// Retrieves all Locations representations.
        /// </summary>
        /// <returns>Action result containing an enumeration of locations</returns>
        /// <example>
        /// GET: /api/v1/location
        /// </example>
        [Authorize]
        public IHttpActionResult Get()
        {
            //IEnumerable<LocationModel>

            var locations = _domainManager.FindAll();

            var models = locations.Select(x => new LocationModel
            {
                Description = x.Description,
                Id = x.Id,
                Latitude = x.Latitude,
                Longitude = x.Longitude
            }).ToList();


            return Ok(models);
        }

        /// <summary>
        /// Retrieves a specific location representation.
        /// </summary>
        /// <param name="id">Unique identifier for the location resource</param>
        /// <returns>Action result containing a representation of the requested location</returns>
        /// <example>
        /// GET: /api/v1/location/123
        /// </example>
        [Authorize]
        public IHttpActionResult Get(int id)
        {
            //LocationModel
            return null;
        }

        /// <summary>
        /// Creates a new location resource.
        /// </summary>
        /// <param name="input">Complete parameters of the location resource</param>
        /// <returns>Action result containing the new resource's permanent URL</returns>
        /// <example>
        /// POST: /api/v1/location
        /// {
        ///   "description": "some location",
        ///   "longitude": "34.02356",
        ///   "latitude": "73.47885"
        /// }
        /// </example>
        [Authorize]
        [HttpPost]
        public IHttpActionResult Post([FromBody]LocationModel input)
        {
            //LocationModel
            return null;
        }

        /// <summary>
        /// Updates an existing location resource.
        /// </summary>
        /// <param name="input">Complete parameters of the location resource</param>
        /// <returns>Action result with status code 204 "no content"</returns>
        /// <example>
        /// PUT: /api/v1/location/123
        /// {
        ///   "description": "some location",
        ///   "longitude": "34.02356",
        ///   "latitude": "73.47885"
        /// }
        /// </example>
        [Authorize]
        [HttpPut]
        public IHttpActionResult Put([FromBody]LocationModel input)
        {
            return null;
        }

        /// <summary>
        /// Retrieves a simplified list representation of all location resources.
        /// </summary>
        /// <returns>Action result containing an enumeration of <see cref="SimpleListItem"/></returns>
        /// <example>
        /// GET: /api/v1/location/simple
        /// </example>
        [Authorize]
        [Route("api/v1/location/simple")]
        public IHttpActionResult GetSimpleList()
        {
            //IEnumerable<SimpleListItem>
            return null;
        }

    }
}
