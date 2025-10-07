using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotesApp.Data;
using NotesApp.Models;

namespace NotesApp.Services
{
    public class NoteService : INoteService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NoteService> _logger;

        public NoteService(ApplicationDbContext context, ILogger<NoteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Note>> GetAllNotesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all notes from database");
                return await _context.Notes
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all notes");
                throw new ApplicationException("An error occurred while retrieving notes. Please try again later.", ex);
            }
        }

        public async Task<Note?> GetNoteByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching note with ID: {NoteId}", id);
                
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid note ID provided: {NoteId}", id);
                    return null;
                }

                return await _context.Notes.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching note with ID: {NoteId}", id);
                throw new ApplicationException($"An error occurred while retrieving note with ID {id}.", ex);
            }
        }

        public async Task<Note> CreateNoteAsync(Note note)
        {
            try
            {
                if (note == null)
                {
                    _logger.LogWarning("Attempted to create a null note");
                    throw new ArgumentNullException(nameof(note), "Note cannot be null");
                }

                _logger.LogInformation("Creating new note with title: {Title}", note.Title);
                
                note.CreatedAt = DateTime.UtcNow;
                _context.Notes.Add(note);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully created note with ID: {NoteId}", note.Id);
                return note;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while creating note");
                throw new ApplicationException("An error occurred while saving the note to the database. Please try again.", ex);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating note");
                throw new ApplicationException("An unexpected error occurred while creating the note.", ex);
            }
        }

        public async Task<Note?> UpdateNoteAsync(Note note)
        {
            try
            {
                if (note == null)
                {
                    _logger.LogWarning("Attempted to update a null note");
                    throw new ArgumentNullException(nameof(note), "Note cannot be null");
                }

                _logger.LogInformation("Updating note with ID: {NoteId}", note.Id);

                var existingNote = await _context.Notes.FindAsync(note.Id);
                if (existingNote == null)
                {
                    _logger.LogWarning("Note with ID {NoteId} not found for update", note.Id);
                    return null;
                }

                existingNote.Title = note.Title;
                existingNote.Content = note.Content;
                existingNote.Priority = note.Priority;
                existingNote.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully updated note with ID: {NoteId}", note.Id);
                return existingNote;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error occurred while updating note with ID: {NoteId}", note?.Id);
                throw new ApplicationException("The note was modified by another user. Please refresh and try again.", ex);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while updating note with ID: {NoteId}", note?.Id);
                throw new ApplicationException("An error occurred while updating the note in the database. Please try again.", ex);
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while updating note with ID: {NoteId}", note?.Id);
                throw new ApplicationException("An unexpected error occurred while updating the note.", ex);
            }
        }

        public async Task<bool> DeleteNoteAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid note ID provided for deletion: {NoteId}", id);
                    return false;
                }

                _logger.LogInformation("Deleting note with ID: {NoteId}", id);

                var note = await _context.Notes.FindAsync(id);
                if (note == null)
                {
                    _logger.LogWarning("Note with ID {NoteId} not found for deletion", id);
                    return false;
                }

                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Successfully deleted note with ID: {NoteId}", id);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error occurred while deleting note with ID: {NoteId}", id);
                throw new ApplicationException("An error occurred while deleting the note from the database. Please try again.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while deleting note with ID: {NoteId}", id);
                throw new ApplicationException("An unexpected error occurred while deleting the note.", ex);
            }
        }
    }
}
