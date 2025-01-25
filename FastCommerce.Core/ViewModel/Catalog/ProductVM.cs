using Core.Entities.Abstract;
using Core.Entities.Enum;
using Core.ViewModel.Generic.Abstracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.ViewModel.Catalog
{
    public class ProductVM : IBaseVM
    {
        public int? Id { get; set; }
        [Required(ErrorMessage = "Title is requered")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Name is requered")]
        public string Name { get; set; }
        [Required(ErrorMessage = "ShortDescription is requered")]
        public string ShortDescription { get; set; }
        [Required(ErrorMessage = "FullDescription is requered")]
        public string FullDescription { get; set; }
        [Required(ErrorMessage = "Html is requered")]
        public string Html { get; set; }
        public string Seo { get; set; }
        public string Mpn { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        [NotMapped]
        public virtual IList<ProductImageVM> ProductImageList { get; set; }
    }
    public class ProductImageVM : BaseEntity, IEntity
    {
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Image is requered")]
        public string Binary { get; set; }
    }
}

