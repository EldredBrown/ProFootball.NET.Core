using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Game
{
    public class GameViewModel
    {
        private int _seasonYear;

        public GameViewModel()
        {
            Game = new EldredBrown.ProFootball.Net.Data.Models.Game();
        }

        public EldredBrown.ProFootball.Net.Data.Models.Game Game { get; set; }

        public int Id
        {
            get { return Game.Id; }
            set { Game.Id = value; }
        }

        [Required]
        public int SeasonYear
        {
            get
            {
                if (Game.SeasonIdNavigation is null)
                {
                    return _seasonYear;
                }
                return Game.SeasonIdNavigation.Id;
            }
            set { _seasonYear = value; }
        }

        [Required]
        public int Week
        {
            get { return Game.Week; }
            set { Game.Week = value; }
        }

        [Required]
        public string GuestName
        {
            get { return Game.GuestName; }
            set { Game.GuestName = value; }
        }

        [Required]
        public int GuestScore
        {
            get { return Game.GuestScore; }
            set { Game.GuestScore = value; }
        }

        [Required]
        public string HostName
        {
            get { return Game.HostName; }
            set { Game.HostName = value; }
        }

        [Required]
        public int HostScore
        {
            get { return Game.HostScore; }
            set { Game.HostScore = value; }
        }

        public bool IsPlayoff
        {
            get { return Game.IsPlayoff; }
            set { Game.IsPlayoff = value; }
        }

        public string Notes
        {
            get { return Game.Notes; }
            set { Game.Notes = value; }
        }
    }
}
