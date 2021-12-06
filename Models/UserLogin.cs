using System.ComponentModel.DataAnnotations;

namespace OpticsStore.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage ="Не указан Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Не указан пароль")]
        public string Password { get; set; }
    }
}