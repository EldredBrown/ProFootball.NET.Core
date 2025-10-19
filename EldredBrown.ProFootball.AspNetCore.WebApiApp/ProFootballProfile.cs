using AutoMapper;
using EldredBrown.ProFootball.AspNetCore.WebApiApp.Models;
using EldredBrown.ProFootball.NETCore.Data.Entities;

namespace EldredBrown.ProFootball.AspNetCore.WebApi
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
            CreateMap<SeasonTeamStanding, SeasonTeamStandingModel>().ReverseMap();
        }
    }
}
