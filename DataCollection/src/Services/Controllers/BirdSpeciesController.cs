using FlightNode.Common.Api.Models;
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
    public class BirdSpeciesController : LoggingController
    {

        private readonly IBirdSpeciesDomainManager _domainManager;

        /// <summary>
        /// Creates a new instance of <see cref="LocationsController"/>.
        /// </summary>
        /// <param name="domainManager">An instance of <see cref="IBirdSpeciesDomainManager"/></param>
        public BirdSpeciesController(IBirdSpeciesDomainManager domainManager)
        {
            if (domainManager == null)
            {
                throw new ArgumentNullException("domainManager");
            }

            _domainManager = domainManager;
        }

        /// <summary>
        /// Retrieves all Work Type representations.
        /// </summary>
        /// <returns>Action result containing an enumeration of work types</returns>
        /// <example>
        /// GET: /api/v1/birdspecies
        /// </example>
        [Authorize]
        public IHttpActionResult Get()
        {
            return WrapWithTryCatch(() =>
            {
                var locations = _domainManager.FindAll();

                var models = locations.Select(x => Map(x));

                return base.Ok(models);
            });
        }

        private static BirdSpeciesModel Map(BirdSpecies x)
        {
            return new BirdSpeciesModel
            {
                CommonAlphaCode = x.CommonAlphaCode,
                CommonName = x.CommonName,
                Family = x.Family,
                SubFamily = x.SubFamily,
                Genus = x.Genus,
                Order = x.Order,
                Species = x.Species,
                Id = x.Id
            };
        }

        /// <summary>
        /// Retrieves a specific work type representation.
        /// </summary>
        /// <param name="id">Unique identifier for the work type resource</param>
        /// <returns>Action result containing a representation of the requested work types</returns>
        /// <example>
        /// GET: /api/v1/birdspecies/123
        /// </example>
        [Authorize]
        public IHttpActionResult Get(int id)
        {
            return WrapWithTryCatch(() =>
            {
                var x = _domainManager.FindById(id);

                var model = Map(x);

                return Ok(model);
            });
        }

        /// <summary>
        /// Creates a new work type resource.
        /// </summary>
        /// <param name="input">Complete parameters of the work type resource</param>
        /// <returns>Action result containing the new resource's permanent URL</returns>
        /// <example>
        /// POST: /api/v1/birdspecies
        /// {
        ///   "description": "some location"
        /// }
        /// </example>
        [Authorize(Roles="Administrator,Coordinator")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]BirdSpeciesModel input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }

            return WrapWithTryCatch(() =>
            {
                BirdSpecies BirdSpecies = Map(input);

                BirdSpecies = _domainManager.Create(BirdSpecies);

                var locationHeader = this.Request
                    .RequestUri
                    .ToString()
                    .AppendPathSegment(BirdSpecies.Id.ToString());

                return Created(locationHeader, BirdSpecies);
            });
        }

        private static BirdSpecies Map(BirdSpeciesModel input)
        {
            return new BirdSpecies
            {
                CommonAlphaCode = input.CommonAlphaCode,
                CommonName = input.CommonName,
                Family = input.Family,
                SubFamily = input.SubFamily,
                Genus = input.Genus,
                Order = input.Order,
                Species = input.Species,
                Id = input.Id
            };
        }

        /// <summary>
        /// Updates an existing work type resource.
        /// </summary>
        /// <param name="input">Complete parameters of the work type resource</param>
        /// <returns>Action result with status code 204 "no content"</returns>
        /// <example>
        /// PUT: /api/v1/birdspecies/123
        /// {
        ///   "description": "some location"
        ///   "id": 3
        /// }
        /// </example>
        [Authorize(Roles = "Administrator,Coordinator")]
        [HttpPut]
        public IHttpActionResult Put([FromBody]BirdSpeciesModel input)
        {
            if (input == null)
            {
                throw new ArgumentNullException();
            }

            return WrapWithTryCatch(() =>
            {
                var location = Map(input);

                _domainManager.Update(location);                

                return NoContent();
            });
        }

        /// <summary>
        /// Retrieves a simplified list representation of all work type resources.
        /// </summary>
        /// <returns>Action result containing an enumeration of <see cref="SimpleListItem"/></returns>
        /// <example>
        /// GET: /api/v1/birdspecies/simple
        /// </example>
        [Authorize]
        [Route("api/v1/birdspecies/simple")]
        public IHttpActionResult GetSimpleList()
        {
            return WrapWithTryCatch(() =>
            {
                var worktTypes = _domainManager.FindAll();

                var models = worktTypes.Select(x => new SimpleListItem
                {
                    Value = x.CommonName + " | <i>" + x.Species + "</i>",
                    Id = x.Id
                });

                return Ok(models);
            });
        }

    }
}
