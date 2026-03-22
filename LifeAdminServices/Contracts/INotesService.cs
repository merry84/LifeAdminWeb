using LifeAdminModels.Models;
using ViewModels;

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

        Task<Note?> GetByIdAsync(int id);

        Task<Note?> GetByIdOwnedAsync(int id, string userId);

        Task AddAsync(Note note);

        Task UpdateAsync(Note note);

        Task DeleteAsync(Note note);
    }
}
