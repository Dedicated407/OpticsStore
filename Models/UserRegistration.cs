using System.ComponentModel.DataAnnotations;

namespace OpticsStore.Models
{
    public class UserRegistration : User
    {
        [Compare("Password", ErrorMessage = "Пароли не совпадают!")]
        public string ConfirmPassword { get; set; }
    }
}