using NotesApp.Models;

namespace NotesApp.Services
{
    public interface INoteService
    {
        Task<List<Note>> GetAllNotesAsync();
        Task<Note?> GetNoteByIdAsync(int id);
        Task<Note> CreateNoteAsync(Note note);
        Task<Note?> UpdateNoteAsync(Note note);
        Task<bool> DeleteNoteAsync(int id);
    }
}
