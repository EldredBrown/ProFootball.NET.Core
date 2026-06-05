using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Provides CRUD access to an external <see cref="Game"/> data store.
    /// </summary>
    public class GameRepository : IGameRepository
    {
        private readonly ProFootballDbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the data store.</param>
        public GameRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public IEnumerable<Game> GetGames()
        {
            return _dbContext.Games
                .Include(s => s.SeasonIdNavigation)
                .ToList();
        }

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Game>> GetGamesAsync()
        {
            return await _dbContext.Games
                .Include(s => s.SeasonIdNavigation)
                .ToListAsync();
        }

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        public Game? GetGame(int id)
        {
            if (_dbContext.Games is null)
            {
                return null;
            }

            return _dbContext.Games
                .Include(s => s.SeasonIdNavigation)
                .FirstOrDefault(c => c.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        public async Task<Game?> GetGameAsync(int id)
        {
            if (_dbContext.Games is null)
            {
                return null;
            }

            return await _dbContext.Games
                .Include(s => s.SeasonIdNavigation)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> entity to add.</param>
        /// <returns>The added <see cref="Game"/> entity.</returns>
        public Game Add(Game game)
        {
            _dbContext.Add(game);

            return game;
        }

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> entity to add.</param>
        /// <returns>The added <see cref="Game"/> entity.</returns>
        public async Task<Game> AddAsync(Game game)
        {
            await _dbContext.AddAsync(game);

            return game;
        }

        /// <summary>
        /// Updates a <see cref="Game"/> entity in the data store.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> to update.</param>
        /// <returns>The updated <see cref="Game"/> entity.</returns>
        public Game Update(Game game)
        {
            if (_dbContext.Games is null)
            {
                return game;
            }

            _dbContext.Update(game);

            return game;
        }

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Game"/> entity.</returns>
        public Game? Delete(int id)
        {
            if (_dbContext.Games is null)
            {
                return null;
            }

            var game = GetGame(id);
            if (game is null)
            {
                return null;
            }

            _dbContext.Games.Remove(game);

            return game;
        }

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Game"/> entity.</returns>
        public async Task<Game?> DeleteAsync(int id)
        {
            if (_dbContext.Games is null)
            {
                return null;
            }

            var game = await GetGameAsync(id);
            if (game is null)
            {
                return null;
            }

            _dbContext.Games.Remove(game);

            return game;
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Game"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public bool GameExists(int id)
        {
            return _dbContext.Games.Any(c => c.Id == id);
        }

        /// <summary>
        /// Checks to verify whether a specific <see cref="Game"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> GameExistsAsync(int id)
        {
            return await _dbContext.Games.AnyAsync(c => c.Id == id);
        }

        public async Task<int> GetMaxWeekForSeasonAsync(int seasonId)
        {
            var games = await GetGamesAsync();
            var gamesForSeason = games.Where(g => g.SeasonId == seasonId);
            var weeks = gamesForSeason.Select(g => g.Week);
            if (weeks.Any())
            {
                return weeks.Max();
            }
            return 0;
        }
    }
}
