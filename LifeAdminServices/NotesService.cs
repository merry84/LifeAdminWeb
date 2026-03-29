using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.EntityFrameworkCore;
using ViewModels.NotesViewModels;

namespace LifeAdminServices
{
    public class NotesService : INotesService
    {
        private readonly ApplicationDbContext db;

        public NotesService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<NoteQueryViewModel> GetAllAsync(
            string userId,
            string? searchTerm,
            int currentPage,
            int notesPerPage)
        {
            var notesQuery = db.Notes
                .AsNoTracking()
                .Where(n => n.OwnerId == userId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string normalizedSearch = searchTerm.ToLower();

                notesQuery = notesQuery.Where(n =>
                    n.Title.ToLower().Contains(normalizedSearch) ||
                    n.Content.ToLower().Contains(normalizedSearch));
            }

            int totalNotes = await notesQuery.CountAsync();

            var notes = await notesQuery
                .OrderByDescending(n => n.CreatedOn)
                .Skip((currentPage - 1) * notesPerPage)
                .Take(notesPerPage)
                .Select(n => new NoteListItemViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    ContentPreview = n.Content.Length > 120
                        ? n.Content.Substring(0, 120) + "..."
                        : n.Content,
                    CreatedOn = n.CreatedOn
                })
                .ToListAsync();

            return new NoteQueryViewModel
            {
                SearchTerm = searchTerm,
                CurrentPage = currentPage,
                NotesPerPage = notesPerPage,
                TotalNotesCount = totalNotes,
                Notes = notes
            };
        }

        public async Task<IEnumerable<Note>> GetMineAsync(string userId)
        {
            return await db.Notes
                .Include(n => n.Owner)
                .Where(n => n.OwnerId == userId)
                .OrderByDescending(n => n.CreatedOn)
                .ToListAsync();
        }

        public async Task<Note?> GetByIdAsync(Guid id)
        {
            return await db.Notes
                .Include(n => n.Owner)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Note?> GetByIdOwnedAsync(Guid id, string userId)
        {
            return await db.Notes
                .Include(n => n.Owner)
                .FirstOrDefaultAsync(n => n.Id == id && n.OwnerId == userId);
        }

        public async Task AddAsync(Note note)
        {
            await db.Notes.AddAsync(note);
            await db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Note note)
        {
            db.Notes.Update(note);
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Note note)
        {
            note.IsDeleted = true;
            note.DeletedOn = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
        public async Task RestoreAsync(Note note)
        {
            note.IsDeleted = false;
            note.DeletedOn = null;
            await db.SaveChangesAsync();
        }
        public async Task<IEnumerable<Note>> GetDeletedAsync()
           => await db.Notes
            .IgnoreQueryFilters()
            .Include(n => n.Owner)
            .Where(n => n.IsDeleted)
            .OrderByDescending(n => n.DeletedOn)
            .ToListAsync();
        public async Task<Note?> GetDeletedByIdAsync(Guid id)
            => await db.Notes
            .IgnoreQueryFilters()
            .Include(n => n.Owner)
            .FirstOrDefaultAsync(n => n.Id == id && n.IsDeleted);        
       
    }
}

