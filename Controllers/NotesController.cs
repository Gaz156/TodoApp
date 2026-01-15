using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using TodoApp.Models.ViewModels;
using TodoApp.Services;
using System.Security.Claims;
using TodoApp.Data;

namespace NotesApp.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly INotesService _notesService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotesController> _logger;

        public NotesController(INotesService notesService,
                              ApplicationDbContext context,
                              ILogger<NotesController> logger)
        {
            _notesService = notesService;
            _context = context;
            _logger = logger;
        }

        private int GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                throw new InvalidOperationException("Invalid user ID format");
            }

            return userId;
        }

        // GET: Notes
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetCurrentUserId();
                var notes = await _notesService.GetUserNotesAsync(userId);
                return View(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading notes");
                TempData["Error"] = "Error loading notes. Please try again.";
                return RedirectToAction("Login", "Account");
            }
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            return View(new NoteViewModel());
        }

        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NoteViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Get current user ID
                    var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userIdString))
                    {
                        TempData["Error"] = "User not found. Please login again.";
                        return RedirectToAction("Login", "Account");
                    }

                    if (!int.TryParse(userIdString, out int userId))
                    {
                        TempData["Error"] = "Invalid user ID.";
                        return RedirectToAction("Login", "Account");
                    }

                    // Check if user exists in database
                    var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
                    if (!userExists)
                    {
                        TempData["Error"] = "User account not found in database.";
                        return RedirectToAction("Login", "Account");
                    }

                    // Create new note
                    var note = new Note
                    {
                        Title = model.Title,
                        Content = model.Content,
                        UserId = userId,
                        CreatedDate = DateTime.UtcNow,
                        ModifiedDate = DateTime.UtcNow
                    };

                    await _notesService.CreateNoteAsync(note);

                    TempData["Success"] = "Note created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database error while creating note");
                    ModelState.AddModelError("", $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating note");
                    ModelState.AddModelError("", $"Error creating note: {ex.Message}");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var note = await _notesService.GetNoteByIdAsync(id, userId);

                if (note == null)
                {
                    TempData["Error"] = "Note not found or you don't have permission to edit it.";
                    return RedirectToAction(nameof(Index));
                }

                var model = new NoteViewModel
                {
                    NoteId = note.NoteId,
                    Title = note.Title,
                    Content = note.Content,
                    CreatedDate = note.CreatedDate,
                    ModifiedDate = note.ModifiedDate
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading note for edit");
                TempData["Error"] = "Error loading note. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Notes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NoteViewModel model)
        {
            if (id != model.NoteId)
            {
                TempData["Error"] = "Note ID mismatch.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = GetCurrentUserId();
                    var note = await _notesService.GetNoteByIdAsync(id, userId);

                    if (note == null)
                    {
                        TempData["Error"] = "Note not found or you don't have permission to edit it.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Check if user exists
                    var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
                    if (!userExists)
                    {
                        TempData["Error"] = "User account not found in database.";
                        return RedirectToAction("Login", "Account");
                    }

                    // Update note
                    note.Title = model.Title;
                    note.Content = model.Content;
                    note.ModifiedDate = DateTime.UtcNow;

                    await _notesService.UpdateNoteAsync(note);

                    TempData["Success"] = "Note updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await NoteExistsAsync(id))
                    {
                        TempData["Error"] = "Note not found.";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Database error while updating note");
                    ModelState.AddModelError("", $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating note");
                    ModelState.AddModelError("", $"Error updating note: {ex.Message}");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var note = await _notesService.GetNoteByIdAsync(id, userId);

                if (note == null)
                {
                    TempData["Error"] = "Note not found or you don't have permission to delete it.";
                    return RedirectToAction(nameof(Index));
                }

                return View(note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading note for deletion");
                TempData["Error"] = "Error loading note. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var note = await _notesService.GetNoteByIdAsync(id, userId);

                if (note == null)
                {
                    TempData["Error"] = "Note not found or you don't have permission to delete it.";
                    return RedirectToAction(nameof(Index));
                }

                // Check if user exists
                var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
                if (!userExists)
                {
                    TempData["Error"] = "User account not found in database.";
                    return RedirectToAction("Login", "Account");
                }

                await _notesService.DeleteNoteAsync(id, userId);

                _logger.LogInformation($"User {userId} deleted note {id}: {note.Title}");

                TempData["Success"] = $"Note '{note.Title}' deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error while deleting note");
                TempData["Error"] = $"Database error: {dbEx.InnerException?.Message ?? dbEx.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting note");
                TempData["Error"] = $"Error deleting note: {ex.Message}";
            }

            return RedirectToAction(nameof(Delete), new { id });
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var note = await _notesService.GetNoteByIdAsync(id, userId);

                if (note == null)
                {
                    TempData["Error"] = "Note not found or you don't have permission to view it.";
                    return RedirectToAction(nameof(Index));
                }

                return View(note);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading note details");
                TempData["Error"] = "Error loading note. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<bool> NoteExistsAsync(int id)
        {
            return await _context.Notes.AnyAsync(e => e.NoteId == id);
        }
    }
}