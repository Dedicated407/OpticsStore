using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpticsStore.Models;

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