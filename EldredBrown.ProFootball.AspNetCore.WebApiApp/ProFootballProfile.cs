using AutoMapper;

using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.Net.Data.Models;

namespace EldredBrown.ProFootball.AspNetCore.WebApiApp
{
    public class ProFootballProfile : Profile
    {
        public ProFootballProfile()
        {
            CreateMap<TeamSeason, LeagueModel>().ReverseMap();
            CreateMap<Team, TeamModel>().ReverseMap();
            CreateMap<Season, SeasonModel>().ReverseMap();
            CreateMap<LeagueSeason, LeagueSeasonModel>().ReverseMap();
            CreateMap<TeamSeason, TeamSeasonModel>().ReverseMap();
            CreateMap<Game, GameModel>().ReverseMap();
            CreateMap<TeamSeasonOpponentProfile, TeamSeasonOpponentProfileModel>().ReverseMap();
            CreateMap<TeamSeasonScheduleTotals, TeamSeasonScheduleTotalsModel>().ReverseMap();
            CreateMap<TeamSeasonScheduleAverages, TeamSeasonScheduleAveragesModel>().ReverseMap();
            CreateMap<StandingsTeamSeason, StandingsTeamSeasonModel>().ReverseMap();
        }
    }
}
