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
    /// Provides control of access to league data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="LeagueController"/> class.
    /// </remarks>
    /// <param name="leagueRepository">The repository by which league data will be accessed.</param>
    /// <param name="sharedRepository">The repository by which shared data resources will be accessed.</param>
    /// <param name="mapper">The AutoMapper object used for object-object mapping.</param>
    /// <param name="linkGenerator">The <see cref="LinkGenerator"/> object used to generate URLs.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class LeagueController(
        ILeagueRepository leagueRepository, ISharedRepository sharedRepository, IMapper mapper,
        LinkGenerator linkGenerator
        ) : ControllerBase
    {
        // GET: api/Leagues
        /// <summary>
        /// Gets a collection of all leagues from the data store.
        /// </summary>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet]
        public async Task<ActionResult<LeagueModel[]>> GetLeagues()
        {
            try
            {
                var leagues = await leagueRepository.GetLeaguesAsync();

                return mapper.Map<LeagueModel[]>(leagues);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // GET: api/Leagues/5
        /// <summary>
        /// Gets a single league from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the league to fetch.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<LeagueModel>> GetLeague(int id)
        {
            try
            {
                var league = await leagueRepository.GetLeagueAsync(id);
                if (league is null)
                {
                    return NotFound();
                }

                return mapper.Map<LeagueModel>(league);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // POST: api/Leagues
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Posts (adds) a new league to the data store.
        /// </summary>
        /// <param name="model">A <see cref="LeagueModel"/> representing the league to add.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<TeamSeason>> PostLeague(LeagueModel model)
        {
            try
            {
                var location = linkGenerator.GetPathByAction("GetLeague", "Leagues", new { id = -1 });
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use Id");
                }

                var league = mapper.Map<League>(model);

                await leagueRepository.AddAsync(league);

                if (await sharedRepository.SaveChangesAsync() > 0)
                {
                    return Created(location, mapper.Map<LeagueModel>(league));
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // PUT: api/Leagues/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Puts (updates) changes to a league in the data store.
        /// </summary>
        /// <param name="id">The Id of the league to update.</param>
        /// <param name="model">A <see cref="LeagueModel"/> representing the league to update.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<LeagueModel>> PutLeague(int id, LeagueModel model)
        {
            try
            {
                var league = await leagueRepository.GetLeagueAsync(id);
                if (league is null)
                {
                    return NotFound($"Could not find league with Id of {id}");
                }

                mapper.Map(model, league);

                if (await sharedRepository.SaveChangesAsync() > 0)
                {
                    return mapper.Map<LeagueModel>(league);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // DELETE: api/Leagues/5
        /// <summary>
        /// Deletes a league from the data store.
        /// </summary>
        /// <param name="id">The Id of the league to delete.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<TeamSeason>> DeleteLeague(int id)
        {
            try
            {
                var league = await leagueRepository.GetLeagueAsync(id);
                if (league is null)
                {
                    return NotFound($"Could not find league with Id of {id}");
                }

                await leagueRepository.DeleteAsync(id);

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
