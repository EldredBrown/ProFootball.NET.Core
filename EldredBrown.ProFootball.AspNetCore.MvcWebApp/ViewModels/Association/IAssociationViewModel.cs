namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Association
{
    interface IAssociationViewModel
    {
        EldredBrown.ProFootball.Net.Data.Models.Association Association { get; set; }

        int Id { get; set; }
        string ShortName { get; set; }
        string LongName { get; set; }
        int FirstSeasonYear { get; set; }
        int? LastSeasonYear { get; set; }
    }
}
