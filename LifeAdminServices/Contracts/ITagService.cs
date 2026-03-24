using LifeAdminModels.Models;

public interface ITagService
{
    Task<IEnumerable<Tag>> GetAllAsync();
}