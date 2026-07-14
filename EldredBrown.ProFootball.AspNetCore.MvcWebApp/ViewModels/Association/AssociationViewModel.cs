using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EldredBrown.ProFootball.AspNetCore.MvcWebApp.ViewModels.Association
{
    public class AssociationViewModel : IAssociationViewModel
    {
        private string _parentName;

        public AssociationViewModel()
        {
            Association = new EldredBrown.ProFootball.Net.Data.Models.Association();
        }

        public EldredBrown.ProFootball.Net.Data.Models.Association Association { get; set; }

        public int Id
        {
            get { return Association.Id; }
            set { Association.Id = value; }
        }

        [Required]
        [DisplayName("Long Name")]
        public string LongName
        {
            get { return Association.LongName; }
            set { Association.LongName = value; }
        }

        [Required]
        [DisplayName("Short Name")]
        public string ShortName
        {
            get { return Association.ShortName; }
            set { Association.ShortName = value; }
        }

        [DisplayName("Parent Name")]
        public string ParentName
        {
            get
            {
                if (Association.ParentIdNavigation is null)
                {
                    return _parentName;
                }
                return Association.ParentIdNavigation.ShortName;
            }
            set { _parentName = value; }
        }

        [Required]
        [DisplayName("First Season")]
        public int FirstSeasonYear
        {
            get{ return Association.FirstSeasonYear; }
            set { Association.FirstSeasonYear = value; }
        }

        [DisplayName("Last Season")]
        public int? LastSeasonYear
        {
            get { return Association.LastSeasonYear; }
            set { Association.LastSeasonYear = value; }
        }
    }
}
