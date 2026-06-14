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
    /// Provides control of access to team season data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TeamSeasonController"/> class.
    /// </remarks>
    /// <param name="teamSeasonRepository">The repository by which team season data will be accessed.</param>
    /// <param name="sharedRepository">The repository by which shared data resources will be accessed.</param>
    /// <param name="mapper">The AutoMapper object used for object-object mapping.</param>
    /// <param name="linkGenerator">The <see cref="LinkGenerator"/> object used to generate URLs.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class TeamSeasonController(
        ITeamSeasonRepository teamSeasonRepository, ISharedRepository sharedRepository, IMapper mapper,
        LinkGenerator linkGenerator
        ) : ControllerBase
    {
        // GET: api/TeamSeasons
        /// <summary>
        /// Gets a collection of all team seasons from the data store.
        /// </summary>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet]
        public async Task<ActionResult<TeamSeasonModel[]>> GetTeamSeasons()
        {
            try
            {
                var teamSeasons = await teamSeasonRepository.GetTeamSeasonsAsync();

                return mapper.Map<TeamSeasonModel[]>(teamSeasons);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // GET: api/TeamSeasons/5
        /// <summary>
        /// Gets a single team season from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the team season to fetch.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamSeasonModel>> GetTeamSeason(int id)
        {
            try
            {
                var teamSeason = await teamSeasonRepository.GetTeamSeasonAsync(id);
                if (teamSeason is null)
                {
                    return NotFound();
                }

                return mapper.Map<TeamSeasonModel>(teamSeason);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // POST: api/TeamSeasons
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Posts (adds) a new team season to the data store.
        /// </summary>
        /// <param name="model">A <see cref="TeamSeasonModel"/> representing the team season to add.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<TeamSeason>> PostTeamSeason(TeamSeasonModel model)
        {
            try
            {
                var location = linkGenerator.GetPathByAction("GetTeamSeason", "TeamSeasons", new { id = -1 });
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use Id");
                }

                var teamSeason = mapper.Map<TeamSeason>(model);

                await teamSeasonRepository.AddAsync(teamSeason);

                if (await sharedRepository.SaveChangesAsync() > 0)
                {
                    return Created(location, mapper.Map<TeamSeasonModel>(teamSeason));
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // PUT: api/TeamSeasons/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Puts (updates) changes to a team season in the data store.
        /// </summary>
        /// <param name="id">The Id of the team season to update.</param>
        /// <param name="model">A <see cref="TeamSeasonModel"/> representing the team season to update.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<TeamSeasonModel>> PutTeamSeason(int id, TeamSeasonModel model)
        {
            try
            {
                var teamSeason = await teamSeasonRepository.GetTeamSeasonAsync(id);
                if (teamSeason is null)
                {
                    return NotFound($"Could not find teamSeason with Id of {id}");
                }

                mapper.Map(model, teamSeason);

                if (await sharedRepository.SaveChangesAsync() > 0)
                {
                    return mapper.Map<TeamSeasonModel>(teamSeason);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // DELETE: api/TeamSeasons/5
        /// <summary>
        /// Deletes a team season from the data store.
        /// </summary>
        /// <param name="id">The Id of the team season to delete.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<TeamSeason>> DeleteTeamSeason(int id)
        {
            try
            {
                var teamSeason = await teamSeasonRepository.GetTeamSeasonAsync(id);
                if (teamSeason is null)
                {
                    return NotFound($"Could not find teamSeason with Id of {id}");
                }

                await teamSeasonRepository.DeleteAsync(id);

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
