using System.Collections.Generic;
using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.Net.Data.Repositories
{
    /// <summary>
    /// Interface for classes that provide access to a <see cref="Association"/> data store.
    /// </summary>
    public interface IAssociationRepository
    {
        /// <summary>
        /// Gets all <see cref="Association"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Association}"/> of all fetched entities.</returns>
        IEnumerable<Association>? GetAssociations();

        /// <summary>
        /// Gets all <see cref="Association"/> entities in the data store.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Association}"/> of all fetched entities.</returns>
        Task<IEnumerable<Association>?> GetAssociationsAsync();

        /// <summary>
        /// Gets a single <see cref="Association"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Association"/> entity.</returns>
        Association? GetAssociation(int id);

        /// <summary>
        /// Gets a single <see cref="Association"/> entity from the data store by Id.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Association"/> entity.</returns>
        Task<Association?> GetAssociationAsync(int id);

        /// <summary>
        /// Gets a single <see cref="Association"/> entity from the data store by Id.
        /// </summary>
        /// <param name="shortName">The short name of the <see cref="Association"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Association"/> entity.</returns>
        Association? GetAssociationByShortName(string shortName);

        /// <summary>
        /// Gets a single <see cref="Association"/> entity from the data store by Id.
        /// </summary>
        /// <param name="shortName">The short name of the <see cref="Association"/> entity to fetch.</param>
        /// <returns>The fetched <see cref="Association"/> entity.</returns>
        Task<Association?> GetAssociationByShortNameAsync(string shortName);

        /// <summary>
        /// Adds a <see cref="Association"/> entity to the data store.
        /// </summary>
        /// <param name="association">The <see cref="Association"/> entity to add.</param>
        /// <returns>The added <see cref="Association"/> entity.</returns>
        Association Add(Association association);

        /// <summary>
        /// Adds a <see cref="Association"/> entity to the data store.
        /// </summary>
        /// <param name="association">The <see cref="Association"/> entity to add.</param>
        /// <returns>The added <see cref="Association"/> entity.</returns>
        Task<Association> AddAsync(Association association);

        /// <summary>
        /// Updates a <see cref="Association"/> entity in the data store.
        /// </summary>
        /// <param name="association">The <see cref="Association"/> to update.</param>
        /// <returns>The updated <see cref="Association"/> entity.</returns>
        Association Update(Association association);

        /// <summary>
        /// Deletes a <see cref="Association"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Association"/> entity.</returns>
        Association? Delete(int id);

        /// <summary>
        /// Deletes a <see cref="Association"/> entity from the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to delete.</param>
        /// <returns>The deleted <see cref="Association"/> entity.</returns>
        Task<Association?> DeleteAsync(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="Association"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        bool AssociationExists(int id);

        /// <summary>
        /// Checks to verify whether a specific <see cref="Association"/> entity exists in the data store.
        /// </summary>
        /// <param name="id">The Id of the <see cref="Association"/> entity to verify.</param>
        /// <returns>
        /// <c>true</c> if the entity with the given Id exists in the data store; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> AssociationExistsAsync(int id);
    }
}
