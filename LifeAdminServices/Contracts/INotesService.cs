using LifeAdminModels.Models;
using ViewModels.NotesViewModels;

namespace LifeAdminServices.Contracts
{
    public interface INotesService
    {
        Task<NoteQueryViewModel> GetAllAsync(
            string userId,
            string? searchTerm,
            int currentPage,
            int notesPerPage);

        Task<IEnumerable<Note>> GetMineAsync(string userId);

        Task<Note?> GetByIdAsync(Guid id);

        Task<Note?> GetByIdOwnedAsync(Guid id, string userId);

        Task AddAsync(Note note);

        Task UpdateAsync(Note note);

        Task DeleteAsync(Note note);

        Task<IEnumerable<Note>> GetDeletedAsync();
        Task<Note?> GetDeletedByIdAsync(Guid id);
        Task RestoreAsync(Note note);
    }
}
