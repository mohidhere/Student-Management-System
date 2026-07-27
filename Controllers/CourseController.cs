using Microsoft.AspNetCore.Mvc;
using studentmanagement.Models;
using studentmanagement.Services;

namespace studentmanagement.Controllers
{
	public class CoursesController : BaseController
	{
		private readonly StudentService _studentService;
		private readonly CourseService _courseService;


		public CoursesController(
	       CourseService courseService,
	       StudentService studentService)
		{
			_courseService = courseService;
			_studentService = studentService;
		}

		// Display All Courses
		public async Task<IActionResult> Index()
		{
			var courses = await _courseService.GetAllAsync();
			return View(courses);
		}

		// GET: Add Course
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Course course)
		{
			if (!ModelState.IsValid)
				return View(course);

			// Remove extra spaces
			course.CourseName = course.CourseName.Trim();

			// Check duplicate (case-insensitive)
			var courses = await _courseService.GetAllAsync();

			bool exists = courses.Any(c =>
				c.CourseName.Equals(course.CourseName, StringComparison.OrdinalIgnoreCase));

			if (exists)
			{
				ModelState.AddModelError("CourseName", "Course already exists.");
				return View(course);
			}

			await _courseService.CreateAsync(course);

			TempData["Success"] = "Course added successfully.";

			return RedirectToAction(nameof(Index));
		}

		public async Task<IActionResult> Delete(string id)
		{
			var course = await _courseService.GetByIdAsync(id);

			if (course == null)
			{
				return NotFound();
			}

			bool assigned = await _studentService.IsCourseAssignedAsync(course.CourseName);

			if (assigned)
			{
				TempData["Error"] = "This course is assigned to one or more students and cannot be deleted.";

				return RedirectToAction(nameof(Index));
			}

			await _courseService.DeleteAsync(id);

			TempData["Success"] = "Course deleted successfully.";

			return RedirectToAction(nameof(Index));
		}
	}
}