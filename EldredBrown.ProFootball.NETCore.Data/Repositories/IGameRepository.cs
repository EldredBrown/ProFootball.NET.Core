using System.Collections.Generic;
using System.Threading.Tasks;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a <see cref="Game"/> data store.
    /// </summary>
    public interface IGameRepository
    {
        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        IEnumerable<Game>? GetGames();

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        Task<IEnumerable<Game>> GetGamesAsync();

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store for the specified season year.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        IEnumerable<Game> GetGamesBySeason(int seasonYear);

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store asynchronously for the specified season year.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        Task<IEnumerable<Game>> GetGamesBySeasonAsync(int seasonYear);

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        Game? GetGame(int id);

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store asynchronously by ID.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        Task<Game?> GetGameAsync(int id);

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> entity to add.</param>
        /// <returns>The added <see cref="Game"/> entity.</returns>
        Game Add(Game game);

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store asynchrously.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> entity to add.</param>
        /// <returns>The added <see cref="Game"/> entity.</returns>
        Task<Game> AddAsync(Game game);

        /// <summary>
        /// Updates a <see cref="Game"/> entity in the data store.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> entity to update.</param>
        /// <returns>The updated <see cref="Game"/> entity.</returns>
        Game Update(Game game);

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Game"/> entity.</returns>
        Game? Delete(int id);

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store asynchronously.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Game"/> entity.</returns>
        Task<Game?> DeleteAsync(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="Game"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given ID exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> GameExists(int id);
    }
}
