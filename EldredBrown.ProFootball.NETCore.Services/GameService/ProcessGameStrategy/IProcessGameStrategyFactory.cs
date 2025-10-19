namespace EldredBrown.ProFootball.NETCore.Services.GameServiceNS.ProcessGameStrategy
{
    public interface IProcessGameStrategyFactory
    {
        /// <summary>
        /// Creates an instance of the <see cref="ProcessGameStrategyBase"/> class.
        /// </summary>
        /// <param name="direction">The <see cref="Direction"/> value used to determine which type of <see cref="ProcessGameStrategyBase"/> to create.</param>
        /// <returns>A <see cref="ProcessGameStrategyBase"/> object corresponding to the specified <see cref="Direction"/> value.</returns>
        ProcessGameStrategyBase CreateStrategy(Direction direction);
    }
}