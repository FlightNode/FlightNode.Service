using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Managers;
using FlightNode.DataCollection.Services.Models;
using FligthNode.Common.Api.Controllers;
using Flurl;
using System;
using System.Linq;
using System.Web.Http;

namespace FlightNode.DataCollection.Domain.Services.Controllers
{
    /// <summary>
    /// API endpoints for managing geographic locations.
    /// </summary>
    public class LocationsController : LoggingController
    {

        private readonly ILocationDomainManager _domainManager;

        /// <summary>
        /// Creates a new instance of <see cref="LocationsController"/>.
        /// </summary>
        /// <param name="domainManager">An instance of <see cref="ILocationDomainManager"/></param>
        public LocationsController(ILocationDomainManager domainManager)
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
        /// GET: /api/v1/locations
        /// </example>
        /// <remarks>
        /// Only Administrators and Project Coordinators may access this endpoint.
        /// </remarks>
        [Authorize(Roles = "Administrator, Coordinator")]
        public IHttpActionResult Get()
        {
            return WrapWithTryCatch(() =>
            {
                var locations = _domainManager.FindAll();

                var models = locations.Select(x => new LocationModel
                {
                    Description = x.Description,
                    Id = x.Id,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude
                });

                return Ok(models);
            });
        }

        /// <summary>
        /// Retrieves a specific location representation.
        /// </summary>
        /// <param name="id">Unique identifier for the location resource</param>
        /// <returns>Action result containing a representation of the requested location</returns>
        /// <example>
        /// GET: /api/v1/locations/123
        /// </example>
        /// <remarks>
        /// Only Administrators and Project Coordinators may access this endpoint.
        /// </remarks>
        [Authorize(Roles = "Administrator, Coordinator")]
        public IHttpActionResult Get(int id)
        {
            return WrapWithTryCatch(() =>
            {
                var x = _domainManager.FindById(id);

                var model = new LocationModel
                {
                    Description = x.Description,
                    Id = x.Id,
                    Latitude = x.Latitude,
                    Longitude = x.Longitude
                };

                return Ok(model);
            });
        }

        /// <summary>
        /// Creates a new location resource.
        /// </summary>
        /// <param name="input">Complete parameters of the location resource</param>
        /// <returns>Action result containing the new resource's permanent URL</returns>
        /// <example>
        /// POST: /api/v1/locations
        /// {
        ///   "description": "some location",
        ///   "longitude": 34.02356,
        ///   "latitude": 73.47885
        /// }
        /// </example>
        /// <remarks>
        /// Only Administrators and Project Coordinators may access this endpoint.
        /// </remarks>
        [Authorize(Roles = "Administrator, Coordinator")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]LocationModel input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }

            return WrapWithTryCatch(() =>
            {
                var location = new Location
                {
                    Description = input.Description,
                    Latitude = input.Latitude,
                    Longitude = input.Longitude
                };

                location = _domainManager.Create(location);

                var locationHeader = this.Request
                    .RequestUri
                    .ToString()
                    .AppendPathSegment(location.Id.ToString());

                return Created(locationHeader, location);
            });
        }

        /// <summary>
        /// Updates an existing location resource.
        /// </summary>
        /// <param name="input">Complete parameters of the location resource</param>
        /// <returns>Action result with status code 204 "no content"</returns>
        /// <example>
        /// PUT: /api/v1/locations/123
        /// {
        ///   "description": "some location",
        ///   "longitude": 34.02356,
        ///   "latitude": 73.47885,
        ///   "id": 123
        /// }
        /// </example>
        /// <remarks>
        /// Only Administrators and Project Coordinators may access this endpoint.
        /// </remarks>
        [Authorize(Roles = "Administrator, Coordinator")]
        [HttpPut]
        public IHttpActionResult Put([FromBody]LocationModel input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }

            return WrapWithTryCatch(() =>
            {
                var location = new Location
                {
                    Description = input.Description,
                    Latitude = input.Latitude,
                    Longitude = input.Longitude,
                    Id = input.Id
                };

                _domainManager.Update(location);                

                return NoContent();
            });
        }

        /// <summary>
        /// Retrieves a simplified list representation of all location resources.
        /// </summary>
        /// <returns>Action result containing an enumeration of <see cref="SimpleListItem"/></returns>
        /// <example>
        /// GET: /api/v1/locations/simple
        /// </example>
        /// <remarks>
        /// Any authenticated user may access this endpoint.
        /// </remarks>
        [Authorize]
        [Route("api/v1/locations/simple")]
        public IHttpActionResult GetSimpleList()
        {
            //IEnumerable<SimpleListItem>

            return WrapWithTryCatch(() =>
            {
                var locations = _domainManager.FindAll();

                var models = locations.Select(x => new SimpleListItem
                {
                    Value = x.Description,
                    Id = x.Id
                });

                return Ok(models);
            });
        }

    }
}
