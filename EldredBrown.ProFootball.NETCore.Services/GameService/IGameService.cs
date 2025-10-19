using System.Threading.Tasks;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.NETCore.Services.GameServiceNS
{
    /// <summary>
    /// Interface for services that will add, edit, and delete game data in the data store.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store.
        /// </summary>
        /// <param name="newGame">The <see cref="Game"/> entity to add to the data store.</param>
        void AddGame(Game newGame);

        /// <summary>
        /// Adds a <see cref="Game"/> entity to the data store asynchronously.
        /// </summary>
        /// <param name="newGame">The <see cref="Game"/> entity to add to the data store.</param>
        Task AddGameAsync(Game newGame);

        /// <summary>
        /// Edits a <see cref="Game"/> entity in the data store.
        /// </summary>
        /// <param name="newGame">The <see cref="Game"/> entity containing data to add to the data store.</param>
        /// <param name="oldGame">The <see cref="Game"/> entity containing data to remove from the data store.</param>
        void EditGame(Game newGame, Game oldGame);

        /// <summary>
        /// Edits a <see cref="Game"/> entity in the data store asynchronously.
        /// </summary>
        /// <param name="newGame">The <see cref="Game"/> entity containing data to add to the data store.</param>
        /// <param name="oldGame">The <see cref="Game"/> entity containing data to remove from the data store.</param>
        Task EditGameAsync(Game newGame, Game oldGame);

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to delete.</param>
        void DeleteGame(int id);

        /// <summary>
        /// Deletes a <see cref="Game"/> entity from the data store asynchronously.
        /// </summary>
        /// <param name="id">The ID of the <see cref="Game"/> entity to delete.</param>
        Task DeleteGameAsync(int id);
    }
}
