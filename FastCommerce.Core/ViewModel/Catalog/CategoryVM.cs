using Core.ViewModel.Generic.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace Core.ViewModel.Catalog
{
    public class CategoryVM : IBaseVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Categorie Name is required")]
        [StringLength(20)]
        public string Name { get; set; }

        public string Html { get; set; }
        public string Seo { get; set; }
    }
}
