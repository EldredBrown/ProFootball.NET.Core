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
    /// Provides control of access to league season data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="LeagueSeasonController"/> class.
    /// </remarks>
    /// <param name="leagueSeasonRepository">The repository by which league season data will be accessed.</param>
    /// <param name="sharedRepository">The repository by which shared data resources will be accessed.</param>
    /// <param name="mapper">The AutoMapper object used for object-object mapping.</param>
    /// <param name="linkGenerator">The <see cref="LinkGenerator"/> object used to generate URLs.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueSeasonController(
        ILeagueSeasonRepository leagueSeasonRepository, ISharedRepository sharedRepository, IMapper mapper,
        LinkGenerator linkGenerator
        ) : ControllerBase
    {
        // GET: api/LeagueSeasons
        /// <summary>
        /// Gets a collection of all league seasons from the data store.
        /// </summary>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet]
        public async Task<ActionResult<LeagueSeasonModel[]>> GetLeagueSeasons()
        {
            try
            {
                var leagueSeasons = await leagueSeasonRepository.GetLeagueSeasonsAsync();

                return mapper.Map<LeagueSeasonModel[]>(leagueSeasons);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // GET: api/LeagueSeasons/5
        /// <summary>
        /// Gets a single league season from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the league season to fetch.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LeagueSeasonModel>> GetLeagueSeason(int id)
        {
            try
            {
                var leagueSeason = await leagueSeasonRepository.GetLeagueSeasonAsync(id);
                if (leagueSeason is null)
                {
                    return NotFound();
                }

                return mapper.Map<LeagueSeasonModel>(leagueSeason);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // POST: api/LeagueSeasons
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Posts (adds) a new league season to the data store.
        /// </summary>
        /// <param name="model">A <see cref="LeagueSeasonModel"/> representing the league season to add.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<LeagueSeason>> PostLeagueSeason(LeagueSeasonModel model)
        {
            try
            {
                var location = linkGenerator.GetPathByAction("GetLeagueSeason", "LeagueSeasons", new { id = -1 });
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use Id");
                }

                var leagueSeason = mapper.Map<LeagueSeason>(model);

                await leagueSeasonRepository.AddAsync(leagueSeason);

                if (await sharedRepository.SaveChangesAsync() > 0)
                {
                    return Created(location, mapper.Map<LeagueSeasonModel>(leagueSeason));
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // PUT: api/LeagueSeasons/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Puts (updates) changes to a league season in the data store.
        /// </summary>
        /// <param name="id">The Id of the league season to update.</param>
        /// <param name="model">A <see cref="LeagueSeasonModel"/> representing the league season to update.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<LeagueSeasonModel>> PutLeagueSeason(int id, LeagueSeasonModel model)
        {
            try
            {
                var leagueSeason = await leagueSeasonRepository.GetLeagueSeasonAsync(id);
                if (leagueSeason is null)
                {
                    return NotFound($"Could not find leagueSeason with Id of {id}");
                }

                mapper.Map(model, leagueSeason);

                if (await sharedRepository.SaveChangesAsync() > 0)
                {
                    return mapper.Map<LeagueSeasonModel>(leagueSeason);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // DELETE: api/LeagueSeasons/5
        /// <summary>
        /// Deletes a league season from the data store.
        /// </summary>
        /// <param name="id">The Id of the league season to delete.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<LeagueSeason>> DeleteLeagueSeason(int id)
        {
            try
            {
                var leagueSeason = await leagueSeasonRepository.GetLeagueSeasonAsync(id);
                if (leagueSeason is null)
                {
                    return NotFound($"Could not find leagueSeason with Id of {id}");
                }

                await leagueSeasonRepository.DeleteAsync(id);

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
