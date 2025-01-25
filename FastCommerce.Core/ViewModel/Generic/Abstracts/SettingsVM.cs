using System.ComponentModel.DataAnnotations;
namespace Core.ViewModel.Generic.Abstracts
{
    public class SettingsVM : IBaseVM
    {
        public int? Id { get; set; }
        [Required(ErrorMessage ="Inform a o corpo do objeto")]
        public string Body { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}
