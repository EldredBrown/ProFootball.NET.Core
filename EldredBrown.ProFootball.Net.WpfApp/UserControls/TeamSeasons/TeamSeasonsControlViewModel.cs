using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EldredBrown.ProFootball.NETCore.Data.Entities;
using EldredBrown.ProFootball.NETCore.Data.Repositories;
using EldredBrown.ProFootball.NETCore.WpfApp.ViewModels;
using EldredBrown.ProFootball.WpfApp;

namespace EldredBrown.ProFootball.NETCore.WpfApp.UserControls.TeamSeasons
{
    public class TeamSeasonsControlViewModel : ViewModelBase, ITeamSeasonsControlViewModel
    {
        private readonly ITeamSeasonRepository _teamSeasonRepository;
        private readonly ITeamSeasonScheduleRepository _teamSeasonScheduleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamSeasonsControlViewModel"/> class.
        /// </summary>
        /// <param name="teamSeasonRepository">
        /// The <see cref="ITeamSeasonRepository"/> object by which team season data will be accessed.
        /// </param>
        /// <param name="teamSeasonScheduleRepository">
        /// The <see cref="ITeamSeasonScheduleProfileRepository"/> object by which team season opponent profile data
        /// will be accessed.
        /// </param>
        /// <param name="teamSeasonScheduleTotalsRepository">
        /// The <see cref="ITeamSeasonScheduleTotalsRepository"/> object by which team season schedule totals data will
        /// be accessed.
        /// </param>
        /// <param name="teamSeasonScheduleAveragesRepository">
        /// The <see cref="ITeamSeasonScheduleAveragesRepository"/> object by which team season schedule averages data
        /// will be accessed.
        /// </param>
        public TeamSeasonsControlViewModel(
            ITeamSeasonRepository teamSeasonRepository,
            ITeamSeasonScheduleRepository teamSeasonScheduleRepository)
        {
            _teamSeasonRepository = teamSeasonRepository;
            _teamSeasonScheduleRepository = teamSeasonScheduleRepository;
        }

        /// <summary>
        /// Gets or sets the teams collection for this <see cref="TeamSeasonsControlViewModel"/> object.
        /// </summary>
        private ReadOnlyCollection<TeamSeason>? _teams;
        public ReadOnlyCollection<TeamSeason>? Teams
        {
            get
            {
                return _teams;
            }
            set
            {
                if (value is null)
                {
                    throw new ArgumentNullException($"{GetType()}.{nameof(Teams)}");
                }
                else if (value != _teams)
                {
                    _teams = value;
                    OnPropertyChanged("Teams");
                    RequestUpdate = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected team for this <see cref="TeamSeasonsControlViewModel"/> object.
        /// </summary>
        private TeamSeason? _selectedTeam;
        public TeamSeason? SelectedTeam
        {
            get
            {
                return _selectedTeam;
            }
            set
            {
                if (value != _selectedTeam)
                {
                    _selectedTeam = value;
                    OnPropertyChanged("SelectedTeam");

                    ViewTeamSchedule();
                }
            }
        }

        /// <summary>
        /// Gets or sets the team season schedule profile collection for this <see cref="TeamSeasonsControlViewModel"/>
        /// object.
        /// </summary>
        private ReadOnlyCollection<TeamSeasonOpponentProfile>? _teamSeasonScheduleProfile;
        public ReadOnlyCollection<TeamSeasonOpponentProfile>? TeamSeasonScheduleProfile
        {
            get
            {
                return _teamSeasonScheduleProfile;
            }
            private set
            {
                if (value != _teamSeasonScheduleProfile)
                {
                    _teamSeasonScheduleProfile = value;
                    OnPropertyChanged("TeamSeasonScheduleProfile");
                }
            }
        }

        /// <summary>
        /// Gets or sets the team season schedule totals collection for this <see cref="TeamSeasonsControlViewModel"/>
        /// object.
        /// </summary>
        private ReadOnlyCollection<TeamSeasonScheduleTotals>? _teamSeasonScheduleTotals;
        public ReadOnlyCollection<TeamSeasonScheduleTotals>? TeamSeasonScheduleTotals
        {
            get
            {
                return _teamSeasonScheduleTotals;
            }
            private set
            {
                if (value != _teamSeasonScheduleTotals)
                {
                    _teamSeasonScheduleTotals = value;
                    OnPropertyChanged("TeamSeasonScheduleTotals");
                }
            }
        }

        /// <summary>
        /// Gets or sets the team season schedule averages collection for this <see cref="TeamSeasonsControlViewModel"/>
        /// object.
        /// </summary>
        private ReadOnlyCollection<TeamSeasonScheduleAverages>? _teamSeasonScheduleAverages;
        public ReadOnlyCollection<TeamSeasonScheduleAverages>? TeamSeasonScheduleAverages
        {
            get
            {
                return _teamSeasonScheduleAverages;
            }
            private set
            {
                if (value != _teamSeasonScheduleAverages)
                {
                    _teamSeasonScheduleAverages = value;
                    OnPropertyChanged("TeamSeasonScheduleAverages");
                }
            }
        }

        /// <summary>
        /// Loads the team seasons view for the selected season.
        /// </summary>
        private DelegateCommand? _viewTeamsCommand;
        public DelegateCommand ViewTeamsCommand
        {
            get
            {
                if (_viewTeamsCommand is null)
                {
                    _viewTeamsCommand = new DelegateCommand(param => ViewTeams());
                }
                return _viewTeamsCommand;
            }
        }
        private void ViewTeams()
        {
            var teamSeasons = _teamSeasonRepository.GetTeamSeasonsBySeason(WpfGlobals.SelectedSeason);
            Teams = new ReadOnlyCollection<TeamSeason>(teamSeasons.ToList());
        }

        /// <summary>
        /// Loads the team season schedule profile, team season schedule totals, and team season schedule averages
        /// views for the selected team.
        /// </summary>
        private DelegateCommand? _viewTeamScheduleCommand;
        public DelegateCommand ViewTeamScheduleCommand
        {
            get
            {
                if (_viewTeamScheduleCommand is null)
                {
                    _viewTeamScheduleCommand = new DelegateCommand(param => ViewTeamSchedule());
                }
                return _viewTeamScheduleCommand;
            }
        }
        private void ViewTeamSchedule()
        {
            if (SelectedTeam is null)
            {
                return;
            }

            var teamName = SelectedTeam.TeamName;
            var seasonYear = SelectedTeam.SeasonYear;

            var teamSeasonOpponentProfiles =
                _teamSeasonScheduleRepository.GetTeamSeasonScheduleProfile(teamName, seasonYear);
            TeamSeasonScheduleProfile =
                new ReadOnlyCollection<TeamSeasonOpponentProfile>(teamSeasonOpponentProfiles.ToList());

            var teamSeasonScheduleTotals =
                _teamSeasonScheduleRepository.GetTeamSeasonScheduleTotals(teamName, seasonYear);
            TeamSeasonScheduleTotals =
                new ReadOnlyCollection<TeamSeasonScheduleTotals>(
                    new List<TeamSeasonScheduleTotals>
                    {
                        teamSeasonScheduleTotals
                    });

            var teamSeasonScheduleAverages =
                _teamSeasonScheduleRepository.GetTeamSeasonScheduleAverages(teamName, seasonYear);
            TeamSeasonScheduleAverages =
                new ReadOnlyCollection<TeamSeasonScheduleAverages>(
                    new List<TeamSeasonScheduleAverages>
                    {
                        teamSeasonScheduleAverages
                    });
        }

        /// <summary>
        /// Refreshes this <see cref="TeamSeasonsControlViewModel"/> object.
        /// </summary>
        public void Refresh()
        {
            ViewTeams();
        }
    }
}
