using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.EntityFrameworkCore;
using ViewModels.DocumentViewModels;

namespace LifeAdminServices
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext db;

        public DocumentService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<DocumentListViewModel>> GetMineAsync(string userId)
            => await db.Documents
                .AsNoTracking()
                .Where(d => d.OwnerId == userId)
                .OrderByDescending(d => d.UploadedOn)
                .Select(d => new DocumentListViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    FileName = d.FileName,
                    ContentType = d.ContentType,
                    FileSize = d.FileSize,
                    UploadedOn = d.UploadedOn
                })
                .ToListAsync();

        public async Task<IEnumerable<DocumentListViewModel>> GetAllAsync()
            => await db.Documents
                .AsNoTracking()
                .Include(d => d.Owner)
                .OrderByDescending(d => d.UploadedOn)
                .Select(d => new DocumentListViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    FileName = d.FileName,
                    ContentType = d.ContentType,
                    FileSize = d.FileSize,
                    UploadedOn = d.UploadedOn,
                    OwnerUserName = d.Owner.UserName
                })
                .ToListAsync();

        public async Task<Document?> GetByIdAsync(Guid id)
            => await db.Documents.FirstOrDefaultAsync(d => d.Id == id);

        public async Task<Document?> GetByIdOwnedAsync(Guid id, string userId)
            => await db.Documents.FirstOrDefaultAsync(d => d.Id == id && d.OwnerId == userId);

        public async Task AddAsync(Document document)
        {
            await db.Documents.AddAsync(document);
            await db.SaveChangesAsync();
        }
        public async Task DeleteAsync(Document document)
        {
            document.IsDeleted = true;
            document.DeletedOn = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }


        public async Task<IEnumerable<DocumentListViewModel>> GetDeletedAsync()
             => await db.Documents
                .IgnoreQueryFilters()
                .Include(d => d.Owner)
                .Where(d => d.IsDeleted)
                .OrderByDescending(d => d.DeletedOn)
                .Select(d => new DocumentListViewModel
                {
                    Id = d.Id,
                    Title = d.Title,
                    FileName = d.FileName,
                    ContentType = d.ContentType,
                    FileSize = d.FileSize,
                    UploadedOn = d.UploadedOn,
                    OwnerUserName = d.Owner.UserName
                })
                .ToListAsync();

        public async Task<Document?> GetDeletedByIdAsync(Guid id)
            => await db.Documents
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted);

        public async Task RestoreAsync(Document document)
        {
            document.IsDeleted = false;
            document.DeletedOn = null;
            await db.SaveChangesAsync();
        }
    }
}