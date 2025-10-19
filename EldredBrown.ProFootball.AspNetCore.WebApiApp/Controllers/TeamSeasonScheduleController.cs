using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Properties;
using EldredBrown.ProFootball.NETCore.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers
{
    /// <summary>
    /// Provides control of access to team season schedule profile data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TeamSeasonScheduleController : ControllerBase
    {
        private readonly ITeamSeasonScheduleRepository _teamSeasonScheduleRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSeasonScheduleProfileController"/> class.
        /// </summary>
        /// <param name="teamSeasonScheduleProfileRepository">The repository by which team season schedule profile data will be accessed.</param>
        /// <param name="mapper">The AutoMapper object used for object-object mapping.</param>
        public TeamSeasonScheduleController(ITeamSeasonScheduleRepository teamSeasonScheduleRepository, IMapper mapper)
        {
            _teamSeasonScheduleRepository = teamSeasonScheduleRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a collection of all team opponent profiles from the data store by team name and season year.
        /// </summary>
        /// <param name="teamName">The name of the team for which team season schedule profile data will be fetched.</param>
        /// <param name="seasonYear">The year of the season for which team season schedule profile data will be fetched.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("profile/{teamName}/{seasonYear}")]
        public async Task<ActionResult<TeamSeasonOpponentProfileModel[]>> GetTeamSeasonScheduleProfile(string teamName,
            int seasonYear)
        {
            try
            {
                var teamSeasonScheduleProfile = 
                    await _teamSeasonScheduleRepository.GetTeamSeasonScheduleProfileAsync(teamName, seasonYear);

                if (!teamSeasonScheduleProfile.Any())
                {
                    return NotFound();
                }

                return _mapper.Map<TeamSeasonOpponentProfileModel[]>(teamSeasonScheduleProfile);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        /// <summary>
        /// Gets a single team season schedule totals entity from the data store by team name and season year.
        /// </summary>
        /// <param name="teamName">The name of the team for which team season schedule totals data will be fetched.</param>
        /// <param name="seasonYear">The year of the season for which team season schedule totals data will be fetched.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("totals/{teamName}/{seasonYear}")]
        public async Task<ActionResult<TeamSeasonScheduleTotalsModel>> GetTeamSeasonScheduleTotals(string teamName,
            int seasonYear)
        {
            try
            {
                var teamSeasonScheduleTotals =
                    await _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotalsAsync(teamName, seasonYear);

                return _mapper.Map<TeamSeasonScheduleTotalsModel>(teamSeasonScheduleTotals);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        /// <summary>
        /// Gets a single team season schedule averages entity from the data store by team name and season year.
        /// </summary>
        /// <param name="teamName">The name of the team for which team season schedule averages data will be fetched.</param>
        /// <param name="seasonYear">The year of the season for which team season schedule averages data will be fetched.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("averages/{teamName}/{seasonYear}")]
        public async Task<ActionResult<TeamSeasonScheduleAveragesModel>> GetTeamSeasonScheduleAverages(string teamName,
            int seasonYear)
        {
            try
            {
                var teamSeasonScheduleAverages =
                    await _teamSeasonScheduleRepository.GetTeamSeasonScheduleAveragesAsync(teamName, seasonYear);

                return _mapper.Map<TeamSeasonScheduleAveragesModel>(teamSeasonScheduleAverages);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }
    }
}
