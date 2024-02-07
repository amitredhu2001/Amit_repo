using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Login_RegisterWithAdo.net_1.Models
{
    public class Register
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string Hobbies { get; set; }
        public List<string> HobbiesList { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string About { get; set; }
        [Required]
        public string Salary { get; set; }
    }
}
