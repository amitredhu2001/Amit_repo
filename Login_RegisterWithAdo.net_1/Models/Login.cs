using System.ComponentModel.DataAnnotations;

namespace Login_RegisterWithAdo.net_1.Models
{
    public class Login
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
