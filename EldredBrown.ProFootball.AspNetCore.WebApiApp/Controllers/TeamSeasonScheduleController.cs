using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using AutoMapper;

using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Properties;
using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers
{
    /// <summary>
    /// Provides control of access to team season schedule profile data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TeamSeasonScheduleProfileController"/> class.
    /// </remarks>
    /// <param name="teamSeasonScheduleProfileRepository">The repository by which team season schedule profile data will be accessed.</param>
    /// <param name="mapper">The AutoMapper object used for object-object mapping.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class TeamSeasonScheduleController(
        ITeamSeasonScheduleRepository teamSeasonScheduleRepository, IMapper mapper
        ) : ControllerBase
    {
        /// <summary>
        /// Gets a collection of all team opponent profiles from the data store by team name and season year.
        /// </summary>
        /// <param name="teamId">The name of the team for which team season schedule profile data will be fetched.</param>
        /// <param name="seasonId">The year of the season for which team season schedule profile data will be fetched.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("profile/{teamId}/{seasonId}")]
        public async Task<ActionResult<TeamSeasonOpponentProfileModel[]>> GetTeamSeasonScheduleProfile(int teamId,
            int seasonId)
        {
            try
            {
                var teamSeasonScheduleProfile = 
                    await teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamId, seasonId);

                if (!teamSeasonScheduleProfile.Any())
                {
                    return NotFound();
                }

                return mapper.Map<TeamSeasonOpponentProfileModel[]>(teamSeasonScheduleProfile);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        /// <summary>
        /// Gets a single team season schedule totals entity from the data store by team name and season year.
        /// </summary>
        /// <param name="teamId">The name of the team for which team season schedule totals data will be fetched.</param>
        /// <param name="seasonId">The year of the season for which team season schedule totals data will be fetched.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("totals/{teamId}/{seasonId}")]
        public async Task<ActionResult<TeamSeasonScheduleTotalsModel>> GetTeamSeasonScheduleTotals(int teamId,
            int seasonId)
        {
            try
            {
                var teamSeasonScheduleTotals =
                    await teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamId, seasonId);

                return mapper.Map<TeamSeasonScheduleTotalsModel>(teamSeasonScheduleTotals);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        /// <summary>
        /// Gets a single team season schedule averages entity from the data store by team name and season year.
        /// </summary>
        /// <param name="teamId">The name of the team for which team season schedule averages data will be fetched.</param>
        /// <param name="seasonId">The year of the season for which team season schedule averages data will be fetched.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("averages/{teamId}/{seasonId}")]
        public async Task<ActionResult<TeamSeasonScheduleAveragesModel>> GetTeamSeasonScheduleAverages(int teamId,
            int seasonId)
        {
            try
            {
                var teamSeasonScheduleAverages =
                    await teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamId, seasonId);

                return mapper.Map<TeamSeasonScheduleAveragesModel>(teamSeasonScheduleAverages);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }
    }
}
