using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using studentmanagement.Configurations;
using studentmanagement.ViewModels;

namespace studentmanagement.Controllers
{
	public class AccountController : Controller
	{
		private readonly AdminLoginSettings _adminLogin;

		public AccountController(IOptions<AdminLoginSettings> adminLogin)
		{
			_adminLogin = adminLogin.Value;
		}

		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Login(LoginViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (model.Email == _adminLogin.Email &&
				model.Password == _adminLogin.Password)
			{
				HttpContext.Session.SetString("AdminEmail", model.Email);
				return RedirectToAction("Index", "Home");
			}

			ViewBag.Error = "Invalid Email or Password.";
			return View(model);
		}

		public IActionResult Logout()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Login");
		}
	}
}