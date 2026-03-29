using LifeAdminModels.Models;
using ViewModels.DocumentViewModels;

namespace LifeAdminServices.Contracts
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentListViewModel>> GetMineAsync(string userId);
        Task<IEnumerable<DocumentListViewModel>> GetAllAsync();
        Task<Document?> GetByIdAsync(Guid id);
        Task<Document?> GetByIdOwnedAsync(Guid id, string userId);
        Task AddAsync(Document document);
        Task DeleteAsync(Document document);
        Task<IEnumerable<DocumentListViewModel>> GetDeletedAsync();
        Task<Document?> GetDeletedByIdAsync(Guid id);
        Task RestoreAsync(Document document);
    }
}