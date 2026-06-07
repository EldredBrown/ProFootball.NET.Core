using System.Collections.Generic;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
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
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        Task<IEnumerable<Game>?> GetGamesAsync();

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <param name="seasonId">The Id of the season for which to fetch games.</param>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        IEnumerable<Game>? GetGamesBySeason(int seasonId);

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <param name="seasonId">The Id of the season for which to fetch games.</param>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        Task<IEnumerable<Game>?> GetGamesBySeasonAsync(int seasonId);

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <param name="seasonId">The Id of the season for which to fetch games.</param>
        /// <param name="week">The week for which to fetch games.</param>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        IEnumerable<Game>? GetGamesBySeasonAndWeek(int seasonId, int week);

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <param name="seasonId">The Id of the season for which to fetch games.</param>
        /// <param name="week">The week for which to fetch games.</param>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        Task<IEnumerable<Game>?> GetGamesBySeasonAndWeekAsync(int seasonId, int week);

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        Game? GetGame(int id);

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        Task<Game?> GetGameAsync(int id);

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store by season, week, guest team, and host team.
        /// </summary>
        /// <param name="seasonId">The Id of the season for which to fetch a game.</param>
        /// <param name="week">The week for which to fetch a game.</param>
        /// <param name="guestName">The name of the guest team for which to fetch a game.</param>
        /// <param name="hostName">The name of the host team for which to fetch a game.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        Game? GetGameBySeasonWeekGuestAndHost(int seasonId, int week, string guestName, string hostName);

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store by season, week, guest team, and host team.
        /// </summary>
        /// <param name="seasonId">The Id of the season for which to fetch a game.</param>
        /// <param name="week">The week for which to fetch a game.</param>
        /// <param name="guestName">The name of the guest team for which to fetch a game.</param>
        /// <param name="hostName">The name of the host team for which to fetch a game.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        Task<Game?> GetGameBySeasonWeekGuestAndHostAsync(int seasonId, int week, string guestName, string hostName);

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> entity to add.</param>
        /// <returns>The added <see cref="Game"/> entity.</returns>
        Game Add(Game game);

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> entity to add.</param>
        /// <returns>The added <see cref="Game"/> entity.</returns>
        Task<Game> AddAsync(Game game);

        /// <summary>
        /// Updates a <see cref="Game"/> entity in the data store.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> to update.</param>
        /// <returns>The updated <see cref="Game"/> entity.</returns>
        Game Update(Game game);

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Game"/> entity.</returns>
        Game? Delete(int id);

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Game"/> entity.</returns>
        Task<Game?> DeleteAsync(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="Game"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        bool GameExists(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="Game"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> GameExistsAsync(int id);

        Task<int> GetMaxWeekForSeasonAsync(int seasonId);
    }
}
