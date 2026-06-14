using System;
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
    /// Provides control of access to season standings data.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SeasonStandingsController"/> class.
    /// </remarks>
    /// <param name="seasonStandingsRepository">The repository by which season standings data will be accessed.</param>
    /// <param name="mapper">The AutoMapper object used for object-object mapping.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class SeasonStandingsController(ISeasonStandingsRepository seasonStandingsRepository, IMapper mapper)
        : ControllerBase
    {
        // GET: api/SeasonStandings/1920
        /// <summary>
        /// Gets the season standings from the data store by season year.
        /// </summary>
        /// <param name="seasonYear">The year of the season for which season standings data will be fetched.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("{seasonYear}")]
        public async Task<ActionResult<StandingsTeamSeasonModel[]>> GetSeasonStandings(int seasonYear)
        {
            try
            {
                var seasonStandings = await seasonStandingsRepository.GetSeasonStandingsAsync(seasonYear);

                return mapper.Map<StandingsTeamSeasonModel[]>(seasonStandings);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }
    }
}
