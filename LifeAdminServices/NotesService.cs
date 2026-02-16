using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices.Contracts;
using Microsoft.EntityFrameworkCore;
using ViewModels;

namespace LifeAdminServices
{
    public class NotesService : INotesService
    {
        private readonly ApplicationDbContext db;

        public NotesService(ApplicationDbContext db) => this.db = db;

        public async Task<IEnumerable<NoteListItemViewModel>> GetMineAsync(string userId)
            => await db.Notes
                .Where(n => n.OwnerId == userId)
                .OrderByDescending(n => n.Id)
                .Select(n => new NoteListItemViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    ContentPreview = n.Content.Length > 120 ? n.Content.Substring(0, 120) + "…" : n.Content
                })
                .ToListAsync();

        public async Task CreateAsync(NoteFormViewModel model, string userId)
        {
            var note = new Note
            {
                Title = model.Title,
                Content = model.Content,
                OwnerId = userId
            };

            db.Notes.Add(note);
            await db.SaveChangesAsync();
        }

        public async Task<NoteFormViewModel?> GetForEditAsync(int id, string userId)
            => await db.Notes
                .Where(n => n.Id == id && n.OwnerId == userId)
                .Select(n => new NoteFormViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content
                })
                .FirstOrDefaultAsync();

        public async Task EditAsync(NoteFormViewModel model, string userId)
        {
            var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == model.Id && n.OwnerId == userId);
            if (note == null) return;

            note.Title = model.Title;
            note.Content = model.Content;

            await db.SaveChangesAsync();
        }

        public Task<bool> ExistsOwnedAsync(int id, string userId)
            => db.Notes.AnyAsync(n => n.Id == id && n.OwnerId == userId);

        public async Task DeleteAsync(int id, string userId)
        {
            var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == id && n.OwnerId == userId);
            if (note == null) return;

            db.Notes.Remove(note);
            await db.SaveChangesAsync();
        }
    }
}
