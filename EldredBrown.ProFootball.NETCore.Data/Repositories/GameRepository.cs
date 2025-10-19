using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
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
        /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
        public GameRepository(ProFootballDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public IEnumerable<Game>? GetGames()
        {
            return _dbContext.Games;
        }

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Game>> GetGamesAsync()
        {
            return await _dbContext.Games.ToListAsync();
        }

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store for the specified season year.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public IEnumerable<Game> GetGamesBySeason(int seasonYear)
        {
            return GetGames().Where(g => g.SeasonYear == seasonYear);
        }

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store asynchronously for the specified season year.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Game>> GetGamesBySeasonAsync(int seasonYear)
        {
            return (await GetGamesAsync()).Where(g => g.SeasonYear == seasonYear);
        }

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        public Game? GetGame(int id)
        {
            if (_dbContext.Games is null)
            {
                return null;
            }

            return _dbContext.Games.Find(id);
        }

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store asynchronously by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        public async Task<Game?> GetGameAsync(int id)
        {
            if (_dbContext.Games is null)
            {
                return null;
            }

            return await _dbContext.Games.FindAsync(id);
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
        /// Adds a <see cref="Game"/> entity to the data store asynchrously.
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
        /// <param name="game">The <see cref="Game"/> entity to update.</param>
        /// <returns>The updated <see cref="Game"/> entity.</returns>
        public Game Update(Game game)
        {
            if (_dbContext.Games is null)
            {
                return game;
            }

            var entity = _dbContext.Games.Attach(game);
            entity.State = EntityState.Modified;

            return game;
        }

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to delete.</param>
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
        /// Deletes a <see cref="Game"/> entity from the data store asynchronously.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to delete.</param>
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
        /// <param name="id">The ID of the <see cref="Game"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given ID exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> GameExists(int id)
        {
            return await _dbContext.Games.AnyAsync(g => g.ID == id);
        }
    }
}
