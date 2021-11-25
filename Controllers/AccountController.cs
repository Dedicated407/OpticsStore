using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpticsStore.Models;

namespace OpticsStore.Controllers
{
    [Route("Account")]
    public class AccountController : Controller
    {
        private readonly IUserRepository _repository;

        public AccountController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("Login")]
        public IActionResult Login() => View();

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            User user = _repository.FindUser(userLogin.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                await Authenticate(user);
                return RedirectToAction("MainPage", "Store");
            }
            ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            
            return View(userLogin);
        }

        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegistration userRegister)
        {
            if (_repository.FindUser(userRegister.Email) != null)
            {
                ModelState.AddModelError("", "Логин занят!");
                return View(userRegister);
            }

            if (userRegister.Password == userRegister.ConfirmPassword)
            {
                User user = userRegister;
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _repository.CreateUser(user);

                await Authenticate(userRegister);

                return RedirectToAction("MainPage", "Store");
            }
            
            ModelState.AddModelError("", "Пароли не совпадают!");
            return View(userRegister);
        }
        
        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, user.Email)
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie");
            
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));
        }
        
        [HttpGet]
        public RedirectToActionResult Logout()
        {
            HttpContext.SignOutAsync("Cookies");
            return RedirectToActionPermanent("Login", "Account");
        }
    }
}