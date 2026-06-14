using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.Net.Services.ProcessGameStrategy
{
    /// <summary>
    /// A factory class for the creation of subclass instances of the <see cref="ProcessGameStrategyBase"/> class.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ProcessGameStrategyFactory"/> class.
    /// </remarks>
    /// <param name="teamSeasonRepository">The repository by which team season data will be accessed.</param>
    public class ProcessGameStrategyFactory(
        ITeamRepository teamRepository,
        ITeamSeasonRepository teamSeasonRepository
        ) : IProcessGameStrategyFactory
    {
        /// <summary>
        /// Creates a subclass instance of the <see cref="ProcessGameStrategyBase"/> class.
        /// </summary>
        /// <param name="direction">The <see cref="Direction"/> value used to determine which type of <see cref="ProcessGameStrategyBase"/> to create.</param>
        /// <returns>A <see cref="ProcessGameStrategyBase"/> object corresponding to the specified <see cref="Direction"/> value.</returns>
        public ProcessGameStrategyBase CreateStrategy(Direction direction)
        {
            ProcessGameStrategyBase processGameStrategy = direction switch
            {
                Direction.Up => new AddGameStrategy(teamRepository, teamSeasonRepository),
                Direction.Down => new SubtractGameStrategy(teamRepository, teamSeasonRepository),
                _ => NullGameStrategy.Instance
            };

            return processGameStrategy;
        }
    }
}
