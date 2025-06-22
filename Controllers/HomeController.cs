using Demo.Database;
using Demo.Interfaces;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _dbContext;
        private readonly IAuthenticationService _authService;

        public HomeController(ILogger<HomeController> logger, DBContext dBContext, IAuthenticationService authService)
        {
            _logger = logger;
            _dbContext = dBContext;
            _authService = authService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users model)
        {
            var result = _authService.LoginAsync(model.Email, model.PasswordHash);
            if (result.success)
            {
                // Set authentication cookie/session
                HttpContext.Session.SetString("IsAuthorized", "true");
                return RedirectToAction("Index", "Task");
            }

            TempData["Error"] = result.errorMessage;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Users model)
        {
            var result = await _authService.CreateAccountAsync(model);

            if (result.success)
            {
                HttpContext.Session.SetString("IsAuthorized", "true");
                return RedirectToAction("Index", "Task");
            }

            TempData["Error"] = result.errorMessage;
            return RedirectToAction("Index", "Home");
        }
    }
}
