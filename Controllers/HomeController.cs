using Microsoft.AspNetCore.Mvc;
using studentmanagement.Filters;
using studentmanagement.Models;
using studentmanagement.Services;
using System.Diagnostics;

namespace studentmanagement.Controllers
{

	public class HomeController : BaseController
	{
		private readonly ILogger<HomeController> _logger;
		private readonly StudentService _studentService;
		private readonly BatchService _batchService;
		private readonly SectionService _sectionService;

		public HomeController(
			ILogger<HomeController> logger,
			StudentService studentService,
			BatchService batchService,
			SectionService sectionService)
		{
			_logger = logger;
			_studentService = studentService;
			_batchService = batchService;
			_sectionService = sectionService;
		}

		public async Task<IActionResult> Index()
		{
			var students = await _studentService.GetAllAsync();
			var batchLabels = students
			.Where(s => !string.IsNullOrWhiteSpace(s.Batch))
			.GroupBy(s => s.Batch)
			.Select(g => g.Key)
			.ToList();

			var batchCounts = students
				.Where(s => !string.IsNullOrWhiteSpace(s.Batch))
				.GroupBy(s => s.Batch)
				.Select(g => g.Count())
				.ToList();

			ViewBag.BatchLabels = System.Text.Json.JsonSerializer.Serialize(batchLabels);
			ViewBag.BatchChart = System.Text.Json.JsonSerializer.Serialize(batchCounts);
			var batches = await _batchService.GetAllAsync();
			var sections = await _sectionService.GetAllAsync();

			ViewBag.TotalStudents = students.Count;
			ViewBag.TotalBatches = batches.Count;
			ViewBag.TotalSections = sections.Count;

			// Pending Assignments
			ViewBag.PendingAssignments = students.Count(s =>
				string.IsNullOrWhiteSpace(s.Batch) ||
				string.IsNullOrWhiteSpace(s.Section));

			// Recent Students
			ViewBag.RecentStudents = students
				.OrderByDescending(s => s.RegistrationDate)
				.Take(5)
				.ToList();

			// Gender Statistics
			ViewBag.MaleStudents = await _studentService.GetMaleCountAsync();
			ViewBag.FemaleStudents = await _studentService.GetFemaleCountAsync();


			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel
			{
				RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
			});
		}
	}
}