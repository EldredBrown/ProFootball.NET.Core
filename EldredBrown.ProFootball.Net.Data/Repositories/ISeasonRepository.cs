using System.Collections.Generic;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a <see cref="Season"/> data store.
    /// </summary>
    public interface ISeasonRepository
    {
        /// <summary>
        /// Gets all <see cref="Season"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Season}"/> of all fetched entities.</returns>
        IEnumerable<Season>? GetSeasons();

        /// <summary>
        /// Gets all <see cref="Season"/> entities in the data store asynchronously.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Season}"/> of all fetched entities.</returns>
        Task<IEnumerable<Season>?> GetSeasonsAsync();

        /// <summary>
        /// Gets a single <see cref="Season"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The year of the <see cref="Season"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Season"/> entity.</returns>
        Season? GetSeason(int id);

        /// <summary>
        /// Gets a single <see cref="Season"/> entity from the data store asynchronously by Id.
        /// </summary>
        /// <param name="id">The year of the <see cref="Season"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Season"/> entity.</returns>
        Task<Season?> GetSeasonAsync(int id);

        /// <summary>
        /// Adds a <see cref="Season"/> entity to the data store.
        /// </summary>
        /// <param name="season">The <see cref="Season"/> entity to add.</param>
        /// <returns>The added <see cref="Season"/> entity.</returns>
        Season Add(Season season);

        /// <summary>
        /// Adds a <see cref="Season"/> entity to the data store.
        /// </summary>
        /// <param name="season">The <see cref="Season"/> entity to add.</param>
        /// <returns>The added <see cref="Season"/> entity.</returns>
        Task<Season> AddAsync(Season season);

        /// <summary>
        /// Updates a <see cref="Season"/> entity in the data store.
        /// </summary>
        /// <param name="season">The <see cref="Season"/> entity to update.</param>
        /// <returns>The updated <see cref="Season"/> entity.</returns>
        Season Update(Season season);

        /// <summary>
        /// Deletes a <see cref="Season"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Season"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Season"/> entity.</returns>
        Season? Delete(int id);

        /// <summary>
        /// Deletes a <see cref="Season"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Season"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Season"/> entity.</returns>
        Task<Season?> DeleteAsync(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="Season"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Season"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        bool SeasonExists(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="Season"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Season"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> SeasonExistsAsync(int id);
    }
}
