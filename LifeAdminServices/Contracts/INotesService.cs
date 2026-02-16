using ViewModels;

namespace LifeAdminServices.Contracts
{
    public interface INotesService
    {
        Task<IEnumerable<NoteListItemViewModel>> GetMineAsync(string userId);
        Task CreateAsync(NoteFormViewModel model, string userId);
        Task<NoteFormViewModel?> GetForEditAsync(int id, string userId);
        Task EditAsync(NoteFormViewModel model, string userId);
        Task<bool> ExistsOwnedAsync(int id, string userId);
        Task DeleteAsync(int id, string userId);
    }
}
