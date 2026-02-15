namespace LifeAdminServices.Contracts
{
    public interface ILayoutService
    {
        Task<int> GetMyTasksCountAsync(string userId);
    }
}
