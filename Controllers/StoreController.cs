using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpticsStore.Infrastructure.Interfaces;
using OpticsStore.Models;
using OpticsStore.ViewModels;

namespace OpticsStore.Controllers
{
    [Route("HomePage")]
    [Authorize]
    public class StoreController : Controller
    {
        private readonly IStoreRepository _repository;

        public StoreController(IStoreRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ViewResult> MainPage() => 
            await Task.Run(() => View(_repository));

        [HttpGet("CreateOrder")]
        public async Task<ViewResult> CreateOrder(CancellationToken cancellationToken)
        {
            var model = new OrderInputViewModel
            {
                Clinics = await _repository.GetClinics(cancellationToken),
                GlassesFrames = await _repository.GetGlassesFrames(cancellationToken),
                Order = new Order { Clinic = new Clinic(), GlassesFrame = new GlassesFrame() }
            };
            return View(model);
        } 
        
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(OrderInputViewModel model, CancellationToken cancellationToken)
        {
            if (model.Order != null && 
                !string.IsNullOrEmpty(model.Order.UserRecipe) && 
                model.Order.ClinicId != 0 &&
                model.Order.GlassesFrameId != 0)
            {
                await _repository.CreateOrder(model.Order, User.Identity?.Name);
                return RedirectToAction("UserOrders", "Store");
            }

            model.Clinics = await _repository.GetClinics(cancellationToken);
            model.GlassesFrames = await _repository.GetGlassesFrames(cancellationToken); 
            
            ModelState.AddModelError("", "Вы ввели не все данные");
            return View(model);
        }

        [HttpGet("Clinics")]
        public async Task<ViewResult> AllClinics(CancellationToken cancellationToken) => 
            View(await _repository.GetClinics(cancellationToken));
        
        [HttpGet("Factories")]
        public async Task<ViewResult> AllFactories(CancellationToken cancellationToken) => 
            View(await _repository.GetFactories(cancellationToken));
        
        [HttpGet("Orders")]
        public async Task<ViewResult> AllOrders(CancellationToken cancellationToken) => 
            View(await _repository.GetAllOrders(cancellationToken));

        [HttpGet("Users")]
        public async Task<ActionResult> AllUsers(string searchString, CancellationToken cancellationToken)
        {
            var users = await  _repository.GetUsers(cancellationToken);

            if (!string.IsNullOrEmpty(searchString))
            {
                users = await _repository.FindUsersByFilter(searchString);
            }
            
            return View(users);
        }
        
        [HttpGet("UserOrders")]
        public async Task<ViewResult> UserOrders(CancellationToken cancellationToken)
        {
            var user = await _repository.FindUser(User.Identity?.Name);
            return View(await _repository.FindUserOrders(user.Id, cancellationToken));
        }
    }
}