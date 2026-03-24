using LifeAdminData;
using LifeAdminModels.Models;
using Microsoft.EntityFrameworkCore;

public class TagService : ITagService
{
    private readonly ApplicationDbContext db;

    public TagService(ApplicationDbContext db)
        => this.db = db;

    public async Task<IEnumerable<Tag>> GetAllAsync()
        => await db.Tags.ToListAsync();
}