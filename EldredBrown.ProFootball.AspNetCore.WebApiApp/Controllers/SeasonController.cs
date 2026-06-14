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
    /// Provides control of access to season data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SeasonController"/> class.
    /// </remarks>
    /// <param name="seasonRepository">The repository by which season data will be accessed.</param>
    /// <param name="sharedRepository">The repository by which shared data resources will be accessed.</param>
    /// <param name="mapper">The AutoMapper object used for object-object mapping.</param>
    /// <param name="linkGenerator">The <see cref="LinkGenerator"/> object used to generate URLs.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonController(
        ISeasonRepository seasonRepository, ISharedRepository sharedRepository, IMapper mapper,
        LinkGenerator linkGenerator
        ) : ControllerBase
    {
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
                var seasons = await seasonRepository.GetSeasonsAsync();

                return mapper.Map<SeasonModel[]>(seasons);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // GET: api/Seasons/5
        /// <summary>
        /// Gets a single season from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the season to fetch.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SeasonModel>> GetSeason(int id)
        {
            try
            {
                var season = await seasonRepository.GetSeasonAsync(id);
                if (season is null)
                {
                    return NotFound();
                }

                return mapper.Map<SeasonModel>(season);
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
                var location = linkGenerator.GetPathByAction("GetSeason", "Seasons", new { id = -1 });
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use Id");
                }

                var season = mapper.Map<Season>(model);

                await seasonRepository.AddAsync(season);

                if (await sharedRepository.SaveChangesAsync() > 0)
                {
                    return Created(location, mapper.Map<SeasonModel>(season));
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
        /// <param name="id">The Id of the season to update.</param>
        /// <param name="model">A <see cref="SeasonModel"/> representing the season to update.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<SeasonModel>> PutSeason(int id, SeasonModel model)
        {
            try
            {
                var season = await seasonRepository.GetSeasonAsync(id);
                if (season is null)
                {
                    return NotFound($"Could not find season with Id of {id}");
                }

                mapper.Map(model, season);

                if (await sharedRepository.SaveChangesAsync() > 0)
                {
                    return mapper.Map<SeasonModel>(season);
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
        /// <param name="id">The Id of the season to delete.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Season>> DeleteSeason(int id)
        {
            try
            {
                var season = await seasonRepository.GetSeasonAsync(id);
                if (season is null)
                {
                    return NotFound($"Could not find season with Id of {id}");
                }

                await seasonRepository.DeleteAsync(id);

                if (await sharedRepository.SaveChangesAsync() > 0)
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
