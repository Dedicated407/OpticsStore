using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpticsStore.Infrastructure.Interfaces;
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
        public async Task<IActionResult> Login() => 
            await Task.Run(View);

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            var user = await _repository.FindUser(userLogin.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                await Authenticate(user);
                return RedirectToAction("MainPage", "Store");
            }
            ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            
            return View(userLogin);
        }

        [HttpGet("Register")]
        public async Task<IActionResult> Register() => 
            await Task.Run(View);
        
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegistration userRegister)
        {
            var user = await _repository.FindUser(userRegister.Email);
            
            if (user != null)
            {
                ModelState.AddModelError("", "Логин занят!");
                return View(userRegister);
            }

            if (string.IsNullOrEmpty(userRegister.Name) || string.IsNullOrEmpty(userRegister.Email))
            {
                ModelState.AddModelError("", "Вы не ввели все данные!");
                return View(userRegister);
            }

            if (!string.IsNullOrEmpty(userRegister.Password) && userRegister.Password == userRegister.ConfirmPassword)
            {
                User newUser = userRegister;
                newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
                await _repository.CreateUser(newUser);

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
        public async Task<RedirectToActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToActionPermanent("Login", "Account");
        }
    }
}