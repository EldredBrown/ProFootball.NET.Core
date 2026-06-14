using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using EldredBrown.ProFootball.AspNetCore.WebApiApp.Properties;
using EldredBrown.ProFootball.Net.Services;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceController"/> class.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController(IWeeklyUpdateService weeklyUpdateService) : ControllerBase
    {
        // POST: api/Services/RunWeeklyUpdate/1920
        /// <summary>
        /// Runs the Weekly Update service.
        /// </summary>
        /// <param name="seasonId">The year for which the weekly update will be run.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPost]
        [Route("RunWeeklyUpdate/{leagueId}/{seasonId}")]
        public async Task<ActionResult> RunWeeklyUpdate(int leagueId, int seasonId)
        {
            try
            {
                await weeklyUpdateService.RunWeeklyUpdate(leagueId, seasonId);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }
    }
}
