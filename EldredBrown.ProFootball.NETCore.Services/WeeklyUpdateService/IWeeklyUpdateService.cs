using System.Threading.Tasks;

namespace EldredBrown.ProFootball.NETCore.Services
{
    /// <summary>
    /// Interface for services to run a weekly update of the pro football database.
    /// </summary>
    public interface IWeeklyUpdateService
    {
        /// <summary>
        /// Runs a weekly update of the data store.
        /// </summary>
        Task RunWeeklyUpdate(int seasonYear);
    }
}
