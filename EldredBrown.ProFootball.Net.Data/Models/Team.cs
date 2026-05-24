using System.Collections.Generic;

namespace EldredBrown.ProFootball.Net.Data.Models;

public partial class Team
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<TeamSeason> TeamSeasons { get; set; } = new List<TeamSeason>();
}
