using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using AutoMapper;

using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Properties;
using EldredBrown.ProFootball.Net.Data.Models;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers
{
    /// <summary>
    /// Provides control of access to association data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AssociationController"/> class.
    /// </remarks>
    /// <param name="associationRepository">The repository by which association data will be accessed.</param>
    /// <param name="sharedRepository">The repository by which shared data resources will be accessed.</param>
    /// <param name="mapper">The AutoMapper object used for object-object mapping.</param>
    /// <param name="linkGenerator">The <see cref="LinkGenerator"/> object used to generate URLs.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class AssociationController(
        IAssociationRepository associationRepository, ISharedRepository sharedRepository, IMapper mapper,
        LinkGenerator linkGenerator
    ) : ControllerBase
    {
        internal readonly IAssociationRepository _associationRepository = associationRepository;
        internal readonly ISharedRepository _sharedRepository = sharedRepository;
        internal readonly IMapper _mapper = mapper;
        internal readonly LinkGenerator _linkGenerator = linkGenerator;

        // GET: api/Associations
        /// <summary>
        /// Gets a collection of all associations from the data store.
        /// </summary>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet]
        public async Task<ActionResult<AssociationModel[]>> GetAssociations()
        {
            try
            {
                var associations = await _associationRepository.GetAssociationsAsync();

                return _mapper.Map<AssociationModel[]>(associations);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // GET: api/Associations/5
        /// <summary>
        /// Gets a single association from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the association to fetch.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AssociationModel>> GetAssociation(int id)
        {
            try
            {
                var association = await _associationRepository.GetAssociationAsync(id);
                if (association is null)
                {
                    return NotFound();
                }

                return _mapper.Map<AssociationModel>(association);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // POST: api/Associations
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Posts (adds) a new association to the data store.
        /// </summary>
        /// <param name="model">A <see cref="AssociationModel"/> representing the association to add.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<TeamSeason>> PostAssociation(AssociationModel model)
        {
            try
            {
                var location = _linkGenerator.GetPathByAction("GetAssociation", "Associations", new { id = -1 });
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use Id");
                }

                var association = _mapper.Map<Association>(model);

                await _associationRepository.AddAsync(association);

                if (await _sharedRepository.SaveChangesAsync() > 0)
                {
                    return Created(location, _mapper.Map<AssociationModel>(association));
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // PUT: api/Associations/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Puts (updates) changes to a association in the data store.
        /// </summary>
        /// <param name="id">The Id of the association to update.</param>
        /// <param name="model">A <see cref="AssociationModel"/> representing the association to update.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<AssociationModel>> PutAssociation(int id, AssociationModel model)
        {
            try
            {
                var association = await _associationRepository.GetAssociationAsync(id);
                if (association is null)
                {
                    return NotFound($"Could not find association with Id of {id}");
                }

                _mapper.Map(model, association);

                if (await _sharedRepository.SaveChangesAsync() > 0)
                {
                    return _mapper.Map<AssociationModel>(association);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // DELETE: api/Associations/5
        /// <summary>
        /// Deletes a association from the data store.
        /// </summary>
        /// <param name="id">The Id of the association to delete.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<TeamSeason>> DeleteAssociation(int id)
        {
            try
            {
                var association = await _associationRepository.GetAssociationAsync(id);
                if (association is null)
                {
                    return NotFound($"Could not find association with Id of {id}");
                }

                await _associationRepository.DeleteAsync(id);

                if (await _sharedRepository.SaveChangesAsync() > 0)
                {
                    return Ok();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }
    }
}
