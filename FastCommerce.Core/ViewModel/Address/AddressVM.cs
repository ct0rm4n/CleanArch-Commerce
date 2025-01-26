using Core.ViewModel.Generic.Abstracts;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.ViewModel.Address
{
    public class AddressVM : IBaseVM
    {
        [NotMapped]
        public int? Id { get; set; }
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
        public int CityId { get; set; }

        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the adress foreign keys
        /// </summary>
        public int? UserId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
    }
    public class CityVM  : IBaseVM
    {
        public int? Id { get; set; }

        public int CityId { get; set; }
        public string Name { get; set; }
        public int Uf { get; set; }
    }
    public class StatesVM : IBaseVM
    {
        public int? Id { get; set; }
        public int UfId { get; set; }
        public string Name { get; set; }
        public string Uf { get; set; }
        public int Region { get; set; }
    }
}
