using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Properties;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers
{
    /// <summary>
    /// Provides control of access to season data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonsController : ControllerBase
    {
        private readonly ISeasonRepository _seasonRepository;
        private readonly ISharedRepository _sharedRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeasonsController"/> class.
        /// </summary>
        /// <param name="seasonRepository">The repository by which season data will be accessed.</param>
        /// <param name="sharedRepository">The repository by which shared data resources will be accessed.</param>
        /// <param name="mapper">The AutoMapper object used for object-object mapping.</param>
        /// <param name="linkGenerator">The <see cref="LinkGenerator"/> object used to generate URLs.</param>
        public SeasonsController(ISeasonRepository seasonRepository, ISharedRepository sharedRepository,
            IMapper mapper, LinkGenerator linkGenerator)
        {
            _seasonRepository = seasonRepository;
            _sharedRepository = sharedRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        // GET: api/Seasons
        /// <summary>
        /// Gets a collection of all seasons from the data store.
        /// </summary>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet]
        public async Task<ActionResult<SeasonModel[]>> GetSeasons()
        {
            try
            {
                var seasons = await _seasonRepository.GetSeasonsAsync();

                return _mapper.Map<SeasonModel[]>(seasons);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // GET: api/Seasons/5
        /// <summary>
        /// Gets a single season from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the season to fetch.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SeasonModel>> GetSeason(int id)
        {
            try
            {
                var season = await _seasonRepository.GetSeasonAsync(id);
                if (season is null)
                {
                    return NotFound();
                }

                return _mapper.Map<SeasonModel>(season);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // POST: api/Seasons
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Posts (adds) a new season to the data store.
        /// </summary>
        /// <param name="model">A <see cref="SeasonModel"/> representing the season to add.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<Season>> PostSeason(SeasonModel model)
        {
            try
            {
                var location = _linkGenerator.GetPathByAction("GetSeason", "Seasons", new { id = -1 });
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use ID");
                }

                var season = _mapper.Map<Season>(model);

                await _seasonRepository.AddAsync(season);

                if (await _sharedRepository.SaveChangesAsync() > 0)
                {
                    return Created(location, _mapper.Map<SeasonModel>(season));
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }

            return BadRequest();
        }

        // PUT: api/Seasons/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Puts (updates) changes to a season in the data store.
        /// </summary>
        /// <param name="id">The ID of the season to update.</param>
        /// <param name="model">A <see cref="SeasonModel"/> representing the season to update.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<SeasonModel>> PutSeason(int id, SeasonModel model)
        {
            try
            {
                var season = await _seasonRepository.GetSeasonAsync(id);
                if (season is null)
                {
                    return NotFound($"Could not find season with ID of {id}");
                }

                _mapper.Map(model, season);

                if (await _sharedRepository.SaveChangesAsync() > 0)
                {
                    return _mapper.Map<SeasonModel>(season);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }

            return BadRequest();
        }

        // DELETE: api/Seasons/5
        /// <summary>
        /// Deletes a season from the data store.
        /// </summary>
        /// <param name="id">The ID of the season to delete.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Season>> DeleteSeason(int id)
        {
            try
            {
                var season = await _seasonRepository.GetSeasonAsync(id);
                if (season is null)
                {
                    return NotFound($"Could not find season with ID of {id}");
                }

                await _seasonRepository.DeleteAsync(id);

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
