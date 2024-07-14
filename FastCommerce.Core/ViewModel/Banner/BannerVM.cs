using Core.ViewModel.Generic.Abstracts;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Core.ViewModel.Banner
{
    public class BannerVM : IBaseVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [Required(ErrorMessage ="You need select one or more file(s)")]
        public string Binary { get; set; }
        [Required(ErrorMessage = "Information to redirect after click are necessary")]
        public string Url { get; set; } = string.Empty;
        public bool Publish { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}
