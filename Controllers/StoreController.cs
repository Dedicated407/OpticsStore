using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public ViewResult MainPage() => View(_repository);

        [HttpGet("CreateOrder")]
        public ViewResult CreateOrder()
        {
            var model = new OrderInputViewModel
            {
                Clinics = _repository.GetClinics(),
                GlassesFrames = _repository.GetGlassesFrames(),
                Order = new Order { Clinic = new Clinic(), GlassesFrame = new GlassesFrame() }
            };
            return View(model);
        } 
        
        [HttpPost("CreateOrder")]
        public IActionResult CreateOrder(OrderInputViewModel model)
        {
            if (ModelState.IsValid)
            {
                _repository.CreateOrder(model.Order, User.Identity?.Name);
                return RedirectToAction("AllOrders", "Store");
            }

            model.Clinics = _repository.GetClinics();
            model.GlassesFrames = _repository.GetGlassesFrames(); 
            return View();
        }

        [HttpGet("Clinics")]
        public ViewResult AllClinics() => View(_repository.GetClinics());
        
        [HttpGet("Factories")]
        public ViewResult AllFactories() => View(_repository.GetFactories());
        
        [HttpGet("Orders")]
        public ViewResult AllOrders() => View(_repository.GetAllOrders());

        [HttpGet("Users")]
        public ViewResult AllUsers() => View(_repository.GetUsers());
    }
}