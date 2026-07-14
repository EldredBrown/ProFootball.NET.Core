using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game
{
    public class GameViewModel()
    {
        private string _leagueName = string.Empty;

        public EldredBrown.ProFootball.Net.Data.Models.Game Game { get; set; } = new EldredBrown.ProFootball.Net.Data.Models.Game();

        public int Id
        {
            get { return Game.Id; }
            set { Game.Id = value; }
        }

        [Required]
        [DisplayName("Season")]
        public int SeasonYear
        {
            get { return Game.SeasonYear; }
            set { Game.SeasonYear = value; }
        }

        [Required]
        [DisplayName("League")]
        public string LeagueName
        {
            get
            {
                if (Game.LeagueIdNavigation is null)
                {
                    return _leagueName;
                }
                return Game.LeagueIdNavigation.ShortName;
            }
            set
            {
                _leagueName = value;
            }
        }

        [Required]
        [DisplayName("Week")]
        public int Week
        {
            get { return Game.Week; }
            set { Game.Week = value; }
        }

        [Required]
        [DisplayName("Guest")]
        public string GuestName
        {
            get { return Game.GuestName; }
            set { Game.GuestName = value; }
        }

        [Required]
        [DisplayName("Guest Score")]
        public int GuestScore
        {
            get { return Game.GuestScore; }
            set { Game.GuestScore = value; }
        }

        [Required]
        [DisplayName("Host")]
        public string HostName
        {
            get { return Game.HostName; }
            set { Game.HostName = value; }
        }

        [Required]
        [DisplayName("Host Score")]
        public int HostScore
        {
            get { return Game.HostScore; }
            set { Game.HostScore = value; }
        }

        [DisplayName("Playoff?")]
        public bool IsPlayoff
        {
            get { return Game.IsPlayoff; }
            set { Game.IsPlayoff = value; }
        }

        [DisplayName("Notes")]
        public string Notes
        {
            get { return Game.Notes; }
            set { Game.Notes = value; }
        }
    }
}
