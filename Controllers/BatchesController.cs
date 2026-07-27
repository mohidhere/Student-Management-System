using Microsoft.AspNetCore.Mvc;
using studentmanagement.Models;
using studentmanagement.Services;
using MongoDB.Driver;

namespace studentmanagement.Controllers
{
	public class BatchesController : BaseController
	{
		private readonly BatchService _batchService;
		private readonly StudentService _studentService;

		public BatchesController(
			BatchService batchService,
			StudentService studentService)
		{
			_batchService = batchService;
			_studentService = studentService;
		}

		// =========================
		// Batch List + Search
		// =========================
		public async Task<IActionResult> Index(string search)
		{
			List<Batch> batches;

			if (string.IsNullOrWhiteSpace(search))
			{
				batches = await _batchService.GetAllAsync();
			}
			else
			{
				batches = await _batchService.SearchAsync(search);
			}

			ViewBag.Search = search;

			return View(batches);
		}

		// =========================
		// Create Batch
		// =========================
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Batch batch)
		{
			if (!ModelState.IsValid)
			{
				return View(batch);
			}

			// Duplicate Batch Check
			if (await _batchService.ExistsAsync(batch.BatchName))
			{
				ModelState.AddModelError("BatchName", "This batch already exists.");
				return View(batch);
			}

			await _batchService.CreateAsync(batch);

			return RedirectToAction(nameof(Index));
		}

		// =========================
		// Edit Batch
		// =========================
		public async Task<IActionResult> Edit(string id)
		{
			var batch = await _batchService.GetByIdAsync(id);

			if (batch == null)
			{
				return NotFound();
			}

			return View(batch);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(Batch batch)
		{
			if (!ModelState.IsValid)
			{
				return View(batch);
			}

			await _batchService.UpdateAsync(batch);

			return RedirectToAction(nameof(Index));
		}

		// =========================
		// Delete Batch
		// =========================
		public async Task<IActionResult> Delete(string id)
		{
			var batch = await _batchService.GetByIdAsync(id);

			if (batch == null)
			{
				return NotFound();
			}

			// Check if any student is assigned to this batch
			var students = await _studentService.GetAllAsync();

			bool isAssigned = students.Any(s => s.Batch == batch.BatchName);

			if (isAssigned)
			{
				TempData["Error"] = "This batch is assigned to one or more students and cannot be deleted.";

				return RedirectToAction(nameof(Index));
			}

			await _batchService.DeleteAsync(id);

			TempData["Success"] = "Batch deleted successfully.";

			return RedirectToAction(nameof(Index));
		}
	}
}