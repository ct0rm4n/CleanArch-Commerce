
using System.ComponentModel.DataAnnotations;

namespace Core.ViewModel.User
{
    public class LoginVM
    {
        
        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password Name is required")]
        //[StringLength(int.MaxValue, MinimumLength = 7)]
        //[RegularExpression(@"^(?:.*[a-z]){7,}$", ErrorMessage = "String length must be greater than or equal 7 characters.")]
        public string Password { get; set; }
    }
}
