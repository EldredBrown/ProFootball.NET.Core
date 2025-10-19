using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Properties;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.Services.GameServiceNS;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp.Controllers
{
    /// <summary>
    /// Provides control of access to game data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameRepository _gameRepository;
        private readonly ISharedRepository _sharedRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;
        private readonly IGameService _gameService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GamesController"/> class.
        /// </summary>
        /// <param name="gameRepository">
        /// The <see cref="IGameRepository"/> by which game data will be accessed.
        /// </param>
        /// <param name="sharedRepository">
        /// The <see cref="ISharedRepository"/> by which shared data resources will be accessed.
        /// </param>
        /// <param name="mapper">
        /// The <see cref="IMapper"/> object used for object-object mapping.
        /// </param>
        /// <param name="linkGenerator">
        /// The <see cref="LinkGenerator"/> object used to generate URLs.
        /// </param>
        /// <param name="gameService">
        /// The <see cref="IGameService"/> object used to process game data.
        /// </param>
        public GamesController(IGameRepository gameRepository, ISharedRepository sharedRepository,
            IMapper mapper, LinkGenerator linkGenerator, IGameService gameService)
        {
            _gameRepository = gameRepository;
            _sharedRepository = sharedRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
            _gameService = gameService;
        }

        // GET: api/Games
        /// <summary>
        /// Gets a collection of all games from the data store.
        /// </summary>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet]
        public async Task<ActionResult<GameModel[]>> GetGames()
        {
            try
            {
                var games = await _gameRepository.GetGamesAsync();

                return _mapper.Map<GameModel[]>(games);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // GET: api/Games/5
        /// <summary>
        /// Gets a single game from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the game to fetch.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GameModel>> GetGame(int id)
        {
            try
            {
                var game = await _gameRepository.GetGameAsync(id);
                if (game is null)
                {
                    return NotFound();
                }

                return _mapper.Map<GameModel>(game);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // POST: api/Games
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Posts (adds) a new game to the data store.
        /// </summary>
        /// <param name="model">A <see cref="GameModel"/> representing the game to add.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<Game>> PostGame(GameModel model)
        {
            try
            {
                var location = _linkGenerator.GetPathByAction("GetGame", "Games", new { id = -1 });
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use ID");
                }

                var game = _mapper.Map<Game>(model);

                await _gameService.AddGameAsync(game);

                if (await _sharedRepository.SaveChangesAsync() > 0)
                {
                    return Created(location, _mapper.Map<GameModel>(game));
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // PUT: api/Games/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        /// <summary>
        /// Puts (updates) changes to a game in the data store.
        /// </summary>
        /// <param name="id">The ID of the game to update.</param>
        /// <param name="model">A <see cref="GameModel"/> representing the game to update.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<GameModel>> PutGame(int id, Dictionary<string, GameModel> models)
        {
            try
            {
                var oldGame = _mapper.Map<Game>(models["oldGame"]);

                var currentGame = await _gameRepository.GetGameAsync(id);
                if (currentGame is null)
                {
                    return NotFound($"Could not find game with ID of {id}");
                }

                _mapper.Map(models["newGame"], currentGame);

                await _gameService.EditGameAsync(currentGame, oldGame);

                if (await _sharedRepository.SaveChangesAsync() > 0)
                {
                    return _mapper.Map<GameModel>(currentGame);
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, Settings.DatabaseFailureString);
            }
        }

        // DELETE: api/Games/5
        /// <summary>
        /// Deletes a game from the data store.
        /// </summary>
        /// <param name="id">The ID of the game to delete.</param>
        /// <returns>A response representing the result of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<Game>> DeleteGame(int id)
        {
            try
            {
                var game = await _gameRepository.GetGameAsync(id);
                if (game is null)
                {
                    return NotFound($"Could not find game with ID of {id}");
                }

                await _gameService.DeleteGameAsync(id);

                if (await _sharedRepository.SaveChangesAsync() > 0)
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
