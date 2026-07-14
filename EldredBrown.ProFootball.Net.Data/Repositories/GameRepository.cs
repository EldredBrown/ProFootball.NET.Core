using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Provides CRUD access to an external <see cref="Game"/> data store.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="GameRepository"/> class.
    /// </remarks>
    /// <param name="dbContext">The <see cref="ProFootballDbContext"/> representing the database.</param>
    public class GameRepository(ProFootballDbContext dbContext) : IGameRepository
    {
        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public IEnumerable<Game>? GetGames()
        {
            return GetGamesDbSetWithNavigationProperties()?.ToList();
        }

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Game>?> GetGamesAsync()
        {
            var games = GetGamesDbSetWithNavigationProperties();
            return games is null ? null : await games.ToListAsync();
        }

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <param name="seasonYear">The Id of the season for which to fetch games.</param>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public IEnumerable<Game>? GetGamesBySeason(int seasonYear)
        {
            return GetGames()?.Where(g => g.SeasonYear == seasonYear).ToList();
        }

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <param name="seasonYear">The Id of the season for which to fetch games.</param>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Game>?> GetGamesBySeasonAsync(int seasonYear)
        {
            var games = await GetGamesAsync();
            return games is null ? null : games.Where(g => g.SeasonYear == seasonYear).ToList();
        }

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <param name="seasonYear">The Id of the season for which to fetch games.</param>
        /// <param name="week">The week for which to fetch games.</param>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public IEnumerable<Game>? GetGamesBySeasonLeagueAndWeek(int seasonYear, int? leagueId, int? week)
        {
            var games = GetGames();
            games = games?.Where(g => g.SeasonYear == seasonYear);

            if (leagueId is null)
            {
                return games?.ToList();
            }
            games = games?.Where(g => g.LeagueId == leagueId);

            if (week is null)
            {
                return games?.ToList();
            }
            games = games?.Where(g => g.Week == week);

            return games?.ToList();
        }

        /// <summary>
        /// Gets all <see cref="Game"/> entities in the data store.
        /// </summary>
        /// <param name="seasonYear">The Id of the season for which to fetch games.</param>
        /// <param name="week">The week for which to fetch games.</param>
        /// <returns>An <see cref="IEnumerable{Game}"/> of all fetched entities.</returns>
        public async Task<IEnumerable<Game>?> GetGamesBySeasonLeagueAndWeekAsync(int seasonYear, int? leagueId, int? week)
        {
            var games = await GetGamesAsync();
            games = games?.Where(g => g.SeasonYear == seasonYear);

            if (leagueId is null)
            {
                return games?.ToList();
            }
            games = games?.Where(g => g.LeagueId == leagueId);

            if (week is null)
            {
                return games?.ToList();
            }
            games = games?.Where(g => g.Week == week);

            return games?.ToList();
        }

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        public Game? GetGame(int id)
        {
            return GetGames()?.FirstOrDefault(g => g.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store asynchronously by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        public async Task<Game?> GetGameAsync(int id)
        {
            return (await GetGamesAsync())?.FirstOrDefault(g => g.Id == id);
        }

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store by season, week, guest team, and host team.
        /// </summary>
        /// <param name="seasonYear">The Id of the season for which to fetch a game.</param>
        /// <param name="week">The week for which to fetch a game.</param>
        /// <param name="guestName">The name of the guest team for which to fetch a game.</param>
        /// <param name="hostName">The name of the host team for which to fetch a game.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        public Game? GetGameBySeasonWeekGuestAndHost(int seasonYear, int week, string guestName, string hostName)
        {
            return GetGames()?.FirstOrDefault(g =>
                g.SeasonYear == seasonYear &&
                g.Week == week &&
                g.GuestName == guestName &&
                g.HostName == hostName);
        }

        /// <summary>
        /// Gets a single <see cref="Game"/> entity from the data store by season, week, guest team, and host team.
        /// </summary>
        /// <param name="seasonYear">The Id of the season for which to fetch a game.</param>
        /// <param name="week">The week for which to fetch a game.</param>
        /// <param name="guestName">The name of the guest team for which to fetch a game.</param>
        /// <param name="hostName">The name of the host team for which to fetch a game.</param>
        /// <returns>The fetched <see cref="Game"/> entity.</returns>
        public async Task<Game?> GetGameBySeasonWeekGuestAndHostAsync(int seasonYear, int week, string guestName, string hostName)
        {
            return (await GetGamesAsync())?.FirstOrDefault(g =>
                g.SeasonYear == seasonYear &&
                g.Week == week &&
                g.GuestName == guestName &&
                g.HostName == hostName);
        }

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> entity to add.</param>
        /// <returns>The added <see cref="Game"/> entity.</returns>
        public Game Add(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);

            if (dbContext.Games is null)
            {
                return game;
            }

            dbContext.Add(game);
            return game;
        }

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> entity to add.</param>
        /// <returns>The added <see cref="Game"/> entity.</returns>
        public async Task<Game> AddAsync(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);

            if (dbContext.Games is null)
            {
                return game;
            }

            await dbContext.AddAsync(game);
            return game;
        }

        /// <summary>
        /// Updates a <see cref="Game"/> entity in the data store.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> entity to update.</param>
        /// <returns>The updated <see cref="Game"/> entity.</returns>
        public Game Update(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);

            if (dbContext.Games is null)
            {
                return game;
            }

            dbContext.Update(game);
            return game;
        }

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Game"/> entity.</returns>
        public Game? Delete(int id)
        {
            if (dbContext.Games is null)
            {
                return null;
            }

            var game = GetGame(id);
            if (game is null)
            {
                return null;
            }

            dbContext.Remove(game);
            return game;
        }

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Game"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Game"/> entity.</returns>
        public async Task<Game?> DeleteAsync(int id)
        {
            if (dbContext.Games is null)
            {
                return null;
            }

            var game = await GetGameAsync(id);
            if (game is null)
            {
                return null;
            }

            dbContext.Remove(game);
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
            return GetGames()?.Any(g => g.Id == id) ?? false;
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
            return (await GetGamesAsync())?.Any(g => g.Id == id) ?? false;
        }

        public async Task<int> GetMaxWeekForSeasonAsync(int seasonYear)
        {
            var games = await GetGamesAsync();
            var gamesForSeason = games?.Where(g => g.SeasonYear == seasonYear);
            var weeks = gamesForSeason?.Select(g => g.Week);
            if (weeks?.Any() == true)
            {
                return weeks.Max();
            }
            return 0;
        }

        private IIncludableQueryable<Game, Association?>? GetGamesDbSetWithNavigationProperties()
        {
            return dbContext.Games?
                .Include(g => g.SeasonYearNavigation)
                .Include(g => g.LeagueIdNavigation);
        }
    }
}
