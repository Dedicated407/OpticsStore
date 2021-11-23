using Microsoft.AspNetCore.Mvc;
using OpticsStore.Models;

namespace OpticsStore.Controllers
{
    [Route("HomePage")]
    public class StoreController : Controller
    {
        private readonly IStoreRepository _repository;

        public StoreController(IStoreRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ViewResult MainPage() => View(_repository);
        
        [HttpGet("Clinics")]
        public ViewResult AllClinics() => View(_repository.GetClinics());
        
        [HttpGet("Factories")]
        public ViewResult AllFactories() => View(_repository.GetFactories());
        
        [HttpGet("Users")]
        public ViewResult AllUsers() => View(_repository.GetUsers());
    }
}