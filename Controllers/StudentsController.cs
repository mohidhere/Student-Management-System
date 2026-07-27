using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using studentmanagement.Models;
using studentmanagement.Services;
using QRCoder;

namespace studentmanagement.Controllers
{
	public class StudentsController : BaseController
	{
		private readonly StudentService _studentService;
		private readonly EmailService _emailService;
		private readonly CourseService _courseService;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly BatchService _batchService;
		private readonly SectionService _sectionService;
		private readonly OtpService _otpService;


		public StudentsController(
			StudentService studentService,
			EmailService emailService,
			CourseService courseService,
			BatchService batchService,
			SectionService sectionService,
			OtpService otpService,
			IWebHostEnvironment webHostEnvironment)
		{
			_studentService = studentService;
			_emailService = emailService;
			_courseService = courseService;
			_batchService = batchService;
			_sectionService = sectionService;
			_webHostEnvironment = webHostEnvironment;
			_otpService = otpService;
		}

		// =========================
		// Student List + Search
		// =========================
		public async Task<IActionResult> Index(string search)
		{
			List<Student> students;

			if (string.IsNullOrWhiteSpace(search))
			{
				students = await _studentService.GetAllAsync();
			}
			else
			{
				students = await _studentService.SearchAsync(search);
			}

			ViewBag.Search = search;

			return View(students);
		}

		// =========================
		// Create Student
		// =========================
		public async Task<IActionResult> Create()
		{
			ViewBag.Courses = await _courseService.GetAllAsync();
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Student student)
		{
			// Set Registration Date
			student.RegistrationDate = DateTime.Now;

			// Calculate Age Automatically
			if (student.DateOfBirth != DateTime.MinValue)
			{
				student.Age = DateTime.Now.Year - student.DateOfBirth.Year;

				if (student.DateOfBirth.Date > DateTime.Now.AddYears(-student.Age))
				{
					student.Age--;
				}
			}

			// Remove validation for controller-generated fields
			ModelState.Remove(nameof(Student.Age));
			ModelState.Remove(nameof(Student.RegistrationDate));
			ModelState.Remove(nameof(Student.StudentCode));

			if (student.Photo == null)
			{
				ModelState.AddModelError("Photo", "Student picture is required.");
			}

			if (!ModelState.IsValid)
			{
				ViewBag.Courses = await _courseService.GetAllAsync();
				return View(student);
			}

			// Image Upload
			string folderPath = Path.Combine(
				_webHostEnvironment.WebRootPath,
				"images",
				"students");

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			string fileName = Guid.NewGuid().ToString()
							  + Path.GetExtension(student.Photo!.FileName);

			string filePath = Path.Combine(folderPath, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await student.Photo.CopyToAsync(stream);
			}

			student.ProfileImage = "/images/students/" + fileName;

			// Save Student
			await _studentService.CreateAsync(student);

			return RedirectToAction(nameof(Index));
		}
		
		// =========================
			// Edit Student
			// =========================
		public async Task<IActionResult> Edit(string id)
		{
			var student = await _studentService.GetByIdAsync(id);

			if (student == null)
				return NotFound();

			ViewBag.Courses = await _courseService.GetAllAsync();

			return View(student);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Student student, IFormFile? Photo)
		{
			Console.WriteLine("EDIT POST HIT");

			// Ignore validation for ProfileImage
			ModelState.Remove("ProfileImage");

			if (!ModelState.IsValid)
			{
				foreach (var error in ModelState)
				{
					foreach (var msg in error.Value.Errors)
					{
						Console.WriteLine($"{error.Key} => {msg.ErrorMessage}");
					}
				}

				ViewBag.Courses = await _courseService.GetAllAsync();
				return View(student);
			}

			if (!student.OtpVerified)
			{
				ModelState.AddModelError("", "Please verify OTP before updating.");
				ViewBag.Courses = await _courseService.GetAllAsync();
				return View(student);
			}

			var oldStudent = await _studentService.GetByIdAsync(student.Id!);

			if (oldStudent == null)
			{
				return NotFound();
			}

			// Preserve values that should never change
			student.StudentCode = oldStudent.StudentCode;
			student.PublicToken = oldStudent.PublicToken;
			student.RegistrationDate = oldStudent.RegistrationDate;

			student.Batch = oldStudent.Batch;
			student.Section = oldStudent.Section;
			// Photo Upload
			if (Photo != null && Photo.Length > 0)
			{
				string folderPath = Path.Combine(
					_webHostEnvironment.WebRootPath,
					"images",
					"students");

				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}

				string fileName = Guid.NewGuid() + Path.GetExtension(Photo.FileName);

				string filePath = Path.Combine(folderPath, fileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await Photo.CopyToAsync(stream);
				}

				student.ProfileImage = "/images/students/" + fileName;

				// Delete old image
				if (!string.IsNullOrEmpty(oldStudent.ProfileImage) &&
					oldStudent.ProfileImage != "/images/default-user.png")
				{
					string oldImage = Path.Combine(
						_webHostEnvironment.WebRootPath,
						oldStudent.ProfileImage.TrimStart('/')
							.Replace('/', Path.DirectorySeparatorChar));

					if (System.IO.File.Exists(oldImage))
					{
						System.IO.File.Delete(oldImage);
					}
				}
			}
			else
			{
				student.ProfileImage = oldStudent.ProfileImage;
			}

			await _studentService.UpdateAsync(student);

			return RedirectToAction(nameof(Index));
		}

		// =========================
		// Assign Batch & Section
		// =========================
		public async Task<IActionResult> Assign(string id)
		{
			var student = await _studentService.GetByIdAsync(id);

			if (student == null)
			{
				return NotFound();
			}

			ViewBag.Batches = await _batchService.GetAllAsync();
			ViewBag.Sections = await _sectionService.GetAllAsync();

			return View(student);
		}

		[HttpPost]
		public async Task<IActionResult> Assign(Student student)
		{
			var oldStudent = await _studentService.GetByIdAsync(student.Id!);

			if (oldStudent == null)
			{
				return NotFound();
			}

			// Keep existing values
			oldStudent.Batch = student.Batch;
			oldStudent.Section = student.Section;

			await _studentService.UpdateAsync(oldStudent);

			return RedirectToAction(nameof(Index));
		}

		// =========================
		// Delete Student
		// =========================
		public async Task<IActionResult> Delete(string id)
		{
			await _studentService.DeleteAsync(id);

			return RedirectToAction(nameof(Index));
		}

		// =========================
		// View Student Profile
		// =========================
		public async Task<IActionResult> Details(string id)
		{
			var student = await _studentService.GetByIdAsync(id);

			if (student == null)
			{
				return NotFound();
			}

			return View(student);
		}

		public async Task<IActionResult> GenerateQr(string id)
		{
			var student = await _studentService.GetByIdAsync(id);

			if (student == null)
				return NotFound();

			string url = $"{Request.Scheme}://{Request.Host}/Students/PublicProfile/{student.PublicToken}";

			using var qrData = QRCodeGenerator.GenerateQrCode(
				url,
				QRCodeGenerator.ECCLevel.Q);

			using var qrCode = new PngByteQRCode(qrData);

			byte[] qrBytes = qrCode.GetGraphic(20);

			return File(qrBytes, "image/png", $"{student.StudentCode}-QR.png");
		}

		[Microsoft.AspNetCore.Authorization.AllowAnonymous]
		[HttpGet("Students/PublicProfile/{token}")]
		public async Task<IActionResult> PublicProfile(string token)
		{
			var student = await _studentService.GetByPublicTokenAsync(token);

			if (student == null)
			{
				return NotFound();
			}

			return View(student);
		}

		public IActionResult Test()
		{
			return Content("Working");
		}

		// =========================
		// Test Email
		// =========================
		public async Task<IActionResult> TestEmail()
		{
			await _emailService.SendEmailAsync(
				"muhammadmoheed0341@gmail.com",
				"Test Email",
				"<h2>Email Successfully Sent From ASP.NET MVC</h2>");

			return Content("Email Sent Successfully");
		}

		// =========================
		// Generate OTP
		// =========================
		private string GenerateOtp()
		{
			Random random = new Random();
			return random.Next(100000, 999999).ToString();
		}

		// =========================
		// Send OTP
		// =========================
		[HttpPost]
		public async Task<IActionResult> SendOtp(string email)
		{
			var success = await _otpService.SendOtpAsync(HttpContext, email);

			return Json(new
			{
				success,
				message = success
					? "OTP sent successfully."
					: "Please enter your email."
			});
		}

		// =========================
		// Verify OTP
		// =========================
		[HttpPost]
		public IActionResult VerifyOtp(string email, string otp)
		{
			bool verified = _otpService.VerifyOtp(HttpContext, email, otp);

			return Json(new
			{
				success = verified,
				message = verified
					? "OTP Verified Successfully"
					: "Invalid OTP"
			});
		}
	}
}