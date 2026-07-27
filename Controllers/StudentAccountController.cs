using Microsoft.AspNetCore.Mvc;
using studentmanagement.Services;
using studentmanagement.ViewModels;

namespace studentmanagement.Controllers
{
	public class StudentAccountController : Controller
	{
		private readonly StudentService _studentService;
		private readonly OtpService _otpService;

		public StudentAccountController(
			StudentService studentService,
			OtpService otpService)
		{
			_studentService = studentService;
			_otpService = otpService;
		}

		// ==========================
		// Student Signup
		// ==========================
		[HttpGet]
		public IActionResult Signup()
		{
			return View();
		}

		// ==========================
		// Send OTP
		// ==========================
		[HttpPost]
		public async Task<IActionResult> SendOtp(StudentSignupViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("Signup", model);
			}

			var student = await _studentService.GetByEmailAsync(model.Email);

			if (student == null)
			{
				ModelState.AddModelError("", "You are not registered by the administrator.");
				return View("Signup", model);
			}

			if (student.HasAccount)
			{
				ModelState.AddModelError("", "Your account already exists. Please login.");
				return View("Signup", model);
			}

			// Save Email in Session
			HttpContext.Session.SetString("SignupEmail", model.Email);

			// Send OTP
			await _otpService.SendOtpAsync(HttpContext, model.Email);

			// Go to Verify OTP Page
			return RedirectToAction(nameof(VerifyOtp));
		}

		// ==========================
		// Verify OTP Page
		// ==========================
		[HttpGet]
		public IActionResult VerifyOtp()
		{
			var email = HttpContext.Session.GetString("SignupEmail");

			if (string.IsNullOrEmpty(email))
			{
				return RedirectToAction(nameof(Signup));
			}

			return View(new StudentSignupViewModel
			{
				Email = email
			});
		}

		// ==========================
		// Verify OTP & Create Account
		// ==========================
		[HttpPost]
		public async Task<IActionResult> VerifyOtp(StudentSignupViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var email = HttpContext.Session.GetString("SignupEmail");

			if (string.IsNullOrEmpty(email))
			{
				return RedirectToAction(nameof(Signup));
			}

			bool verified = _otpService.VerifyOtp(HttpContext, email, model.Otp);

			if (!verified)
			{
				ModelState.AddModelError("", "Invalid OTP.");
				return View(model);
			}

			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

			await _studentService.ActivateStudentAccountAsync(email, hashedPassword);

			// Remove Session
			HttpContext.Session.Remove("SignupEmail");

			TempData["Success"] = "Account created successfully. Please login.";

			return RedirectToAction(nameof(Login));
		}

		// ==========================
		// Student Login
		// ==========================
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(StudentLoginViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var student = await _studentService.GetByEmailAsync(model.Email);

			if (student == null)
			{
				ModelState.AddModelError("", "Invalid Email or Password.");
				return View(model);
			}

			if (!student.HasAccount)
			{
				ModelState.AddModelError("", "Please create your account first.");
				return View(model);
			}

			if (!student.IsActive)
			{
				ModelState.AddModelError("", "Your account is not active.");
				return View(model);
			}

			bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(
				model.Password,
				student.Password);

			if (!isPasswordCorrect)
			{
				ModelState.AddModelError("", "Invalid Email or Password.");
				return View(model);
			}

			// Create Student Session
			HttpContext.Session.SetString("StudentId", student.Id!);
			HttpContext.Session.SetString("StudentEmail", student.Email);
			HttpContext.Session.SetString("StudentName", student.Name);
			HttpContext.Session.SetString("StudentRole", student.Role);
			HttpContext.Session.SetString("PublicToken", student.PublicToken);

			return RedirectToAction("Dashboard");
		}

		// ==========================
		// Forgot Password
		// ==========================
		[HttpGet]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpGet]
		public async Task<IActionResult> Dashboard()
		{
			var studentId = HttpContext.Session.GetString("StudentId");

			if (string.IsNullOrEmpty(studentId))
			{
				return RedirectToAction(nameof(Login));
			}

			var student = await _studentService.GetByIdAsync(studentId);

			if (student == null)
			{
				return RedirectToAction(nameof(Login));
			}

			return View(student);
		}

		[HttpGet]
		public IActionResult Logout()
		{
			HttpContext.Session.Clear();

			return RedirectToAction(nameof(Login));
		}

		// ==========================
		// Send Reset OTP
		// ==========================
		[HttpPost]
		public async Task<IActionResult> SendResetOtp(ForgotPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View("ForgotPassword", model);
			}

			var student = await _studentService.GetByEmailAsync(model.Email);

			if (student == null)
			{
				ModelState.AddModelError("", "Email not found.");
				return View("ForgotPassword", model);
			}

			HttpContext.Session.SetString("ResetEmail", model.Email);

			await _otpService.SendOtpAsync(HttpContext, model.Email);

			return RedirectToAction(nameof(ResetPassword));
		}

		// ==========================
		// Reset Password Page
		// ==========================
		[HttpGet]
		public IActionResult ResetPassword()
		{
			var email = HttpContext.Session.GetString("ResetEmail");

			if (string.IsNullOrEmpty(email))
			{
				return RedirectToAction(nameof(ForgotPassword));
			}

			return View(new ResetPasswordViewModel());
		}

		// ==========================
		// Reset Password
		// ==========================
		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var email = HttpContext.Session.GetString("ResetEmail");

			if (string.IsNullOrEmpty(email))
			{
				return RedirectToAction(nameof(ForgotPassword));
			}

			bool verified = _otpService.VerifyOtp(HttpContext, email, model.Otp);

			if (!verified)
			{
				ModelState.AddModelError("", "Invalid OTP.");
				return View(model);
			}

			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

			await _studentService.UpdatePasswordAsync(email, hashedPassword);

			HttpContext.Session.Remove("ResetEmail");

			TempData["Success"] = "Password updated successfully. Please login.";

			return RedirectToAction(nameof(Login));
		}

		public IActionResult MyPublicProfile()
		{
			var token = HttpContext.Session.GetString("PublicToken");

			if (string.IsNullOrEmpty(token))
			{
				return RedirectToAction("Login");
			}

			return RedirectToAction(
				"PublicProfile",
				"Students",
				new { token = token });
		}
	}
}