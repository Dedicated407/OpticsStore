using Microsoft.AspNetCore.Mvc;
using OpticsStore.Models;

namespace OpticsStore.Controllers
{
    [Route("Index")]
    public class StoreController : Controller
    {
        private readonly IStoreRepository _repository;

        public StoreController(IStoreRepository repository) => _repository = repository;

        [HttpGet]
        public ViewResult AllUsers() => View(_repository.Users);
    }
}