using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Properties;
using EldredBrown.ProFootball.NETCore.Services;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IWeeklyUpdateService _weeklyUpdateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesController"/> class.
        /// </summary>
        public ServicesController(IWeeklyUpdateService weeklyUpdateService)
        {
            _weeklyUpdateService = weeklyUpdateService;
        }

        // POST: api/Services/RunWeeklyUpdate/1920
        /// <summary>
        /// Runs the Weekly Update service.
        /// </summary>
        /// <param name="year">The year for which the weekly update will be run.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPost]
        [Route("RunWeeklyUpdate/{year}")]
        public async Task<ActionResult> RunWeeklyUpdate(int year)
        {
            try
            {
                await _weeklyUpdateService.RunWeeklyUpdate(year);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }
    }
}
