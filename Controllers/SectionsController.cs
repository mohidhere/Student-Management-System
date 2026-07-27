using Microsoft.AspNetCore.Mvc;
using studentmanagement.Models;
using studentmanagement.Services;

namespace studentmanagement.Controllers
{
	public class SectionsController : BaseController
	{
		private readonly SectionService _sectionService;
		private readonly StudentService _studentService;

		public SectionsController(
			SectionService sectionService,
			StudentService studentService)
		{
			_sectionService = sectionService;
			_studentService = studentService;
		}

		// =========================
		// Section List + Search
		// =========================
		public async Task<IActionResult> Index(string search)
		{
			List<Section> sections;

			if (string.IsNullOrWhiteSpace(search))
			{
				sections = await _sectionService.GetAllAsync();
			}
			else
			{
				sections = await _sectionService.SearchAsync(search);
			}

			ViewBag.Search = search;

			return View(sections);
		}

		// =========================
		// Create Section
		// =========================
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Section section)
		{
			if (!ModelState.IsValid)
			{
				return View(section);
			}

			await _sectionService.CreateAsync(section);

			return RedirectToAction(nameof(Index));
		}

		// =========================
		// Edit Section
		// =========================
		public async Task<IActionResult> Edit(string id)
		{
			var section = await _sectionService.GetByIdAsync(id);

			if (section == null)
			{
				return NotFound();
			}

			return View(section);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Section section)
		{
			if (!ModelState.IsValid)
			{
				return View(section);
			}

			await _sectionService.UpdateAsync(section);

			return RedirectToAction(nameof(Index));
		}

		// =========================
		// Delete Section
		// =========================
		public async Task<IActionResult> Delete(string id)
		{
			var section = await _sectionService.GetByIdAsync(id);

			if (section == null)
			{
				return NotFound();
			}

			// Check if any student is assigned to this section
			var students = await _studentService.GetAllAsync();

			bool isAssigned = students.Any(s => s.Section == section.SectionName);

			if (isAssigned)
			{
				TempData["Error"] = "This section is assigned to one or more students and cannot be deleted.";
				return RedirectToAction(nameof(Index));
			}

			await _sectionService.DeleteAsync(id);

			TempData["Success"] = "Section deleted successfully.";

			return RedirectToAction(nameof(Index));
		}
	}
}