using Core.ViewModel.Generic.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace Core.ViewModel.Catalog
{
    public class BlogPostVM : IBaseVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Post Name is required")]
        [StringLength(20)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Post Body is required")]//, MinLength(20)]
        //[StringLength(20)]
        public string Html { get; set; }
        public string Url { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? UserId { get; set; }
        public bool Publish { get; set; }
        public bool Deleted { get; set; }
    }
}
