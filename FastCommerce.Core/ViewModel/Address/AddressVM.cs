using Core.ViewModel.Generic.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace Core.ViewModel.Address
{
    public class AddressVM : IBaseVM
    {
        [Required]
        public string Cep { get; set; }
        [Required]
        public string Street  { get; set; }
        [Required]
        public string Neighborhood { get; set; }
        public string Complement { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public string City { get; set; }
        
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the adress foreign keys
        /// </summary>
        public int? UserId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
    }
}
