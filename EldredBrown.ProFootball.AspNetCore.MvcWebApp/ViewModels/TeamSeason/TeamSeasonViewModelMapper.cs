using System.Threading.Tasks;

using EldredBrown.ProFootball.Net.Data.Repositories;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.TeamSeason
{
    /// <summary>
    /// A class that maps game data to game view models.
    /// </summary>
    public class TeamSeasonViewModelMapper(
        ITeamRepository teamRepository, ISeasonRepository seasonRepository, ILeagueRepository leagueRepository
        ) : ITeamSeasonViewModelMapper
    {
        public TeamSeasonViewModel MapTeamSeasonToViewModel(EldredBrown.ProFootball.Net.Data.Models.TeamSeason teamSeason)
        {
            return new TeamSeasonViewModel { TeamSeason = teamSeason };
        }

        public async Task<EldredBrown.ProFootball.Net.Data.Models.TeamSeason> MapViewModelToTeamSeason(
            TeamSeasonViewModel teamSeasonViewModel
        )
        {
            var teamSeason = teamSeasonViewModel.TeamSeason;

            var team = await teamRepository.GetTeamByNameAsync(teamSeasonViewModel.TeamName);
            teamSeason.TeamId = team is not null ? team.Id : -1;

            var season = await seasonRepository.GetSeasonAsync(teamSeasonViewModel.SeasonYear);
            teamSeason.SeasonId = season is not null ? season.Id : -1;

            var league = await leagueRepository.GetLeagueByShortNameAsync(teamSeasonViewModel.LeagueName);
            teamSeason.LeagueId = league is not null ? league.Id : -1;

            return teamSeason;
        }
    }
}
