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
    /// Provides control of access to team data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TeamController"/> class.
    /// </remarks>
    /// <param name="teamRepository">The repository by which team data will be accessed.</param>
    /// <param name="sharedRepository">The repository by which shared data resources will be accessed.</param>
    /// <param name="mapper">The AutoMapper object used for object-object mapping.</param>
    /// <param name="linkGenerator">The <see cref="LinkGenerator"/> object used to generate URLs.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController(
        ITeamRepository teamRepository, ISharedRepository sharedRepository, IMapper mapper, LinkGenerator linkGenerator
        ) : ControllerBase
    {
        // GET: api/Teams
        /// <summary>
        /// Gets a collection of all teams from the data store.
        /// </summary>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet]
        public async Task<ActionResult<TeamModel[]>> GetTeams()
        {
            try
            {
                var teams = await teamRepository.GetTeamsAsync();

                return mapper.Map<TeamModel[]>(teams);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // GET: api/Teams/5
        /// <summary>
        /// Gets a single team from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the team to fetch.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamModel>> GetTeam(int id)
        {
            try
            {
                var team = await teamRepository.GetTeamAsync(id);
                if (team is null)
                {
                    return NotFound();
                }

                return mapper.Map<TeamModel>(team);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // POST: api/Teams
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Posts (adds) a new team to the data store.
        /// </summary>
        /// <param name="model">A <see cref="TeamModel"/> representing the team to add.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(TeamModel model)
        {
            try
            {
                var location = linkGenerator.GetPathByAction("GetTeam", "Teams", new { id = -1 });
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use Id");
                }

                var team = mapper.Map<Team>(model);

                await teamRepository.AddAsync(team);

                if (await sharedRepository.SaveChangesAsync() > 0)
                {
                    return Created(location, mapper.Map<TeamModel>(team));
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // PUT: api/Teams/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Puts (updates) changes to a team in the data store.
        /// </summary>
        /// <param name="id">The Id of the team to update.</param>
        /// <param name="model">A <see cref="TeamModel"/> representing the team to update.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<TeamModel>> PutTeam(int id, TeamModel model)
        {
            try
            {
                var team = await teamRepository.GetTeamAsync(id);
                if (team is null)
                {
                    return NotFound($"Could not find team with Id of {id}");
                }

                mapper.Map(model, team);

                if (await sharedRepository.SaveChangesAsync() > 0)
                {
                    return mapper.Map<TeamModel>(team);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // DELETE: api/Teams/5
        /// <summary>
        /// Deletes a team from the data store.
        /// </summary>
        /// <param name="id">The Id of the team to delete.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Team>> DeleteTeam(int id)
        {
            try
            {
                var team = await teamRepository.GetTeamAsync(id);
                if (team is null)
                {
                    return NotFound($"Could not find team with Id of {id}");
                }

                await teamRepository.DeleteAsync(id);

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
