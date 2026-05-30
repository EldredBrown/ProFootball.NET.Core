namespace EldredBrown.ProFootball.Net.Data.Models
{
    public partial class TeamSeason
    {
        public decimal? WinningPercentage
        {
            get
            {
                if (Games == 0)
                {
                    return null;
                }
                return (decimal)(2 * Wins + Ties) / (2 * Games);
            }
        }
    }
}
