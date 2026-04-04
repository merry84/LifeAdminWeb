using FluentAssertions;
using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices;

namespace LifeAdmin.Tests.Services
{
    [TestFixture]
    public class DashboardServiceTests
    {
        private ApplicationDbContext dbContext;
        private DashboardService dashboardService;

        [SetUp]
        public void SetUp()
        {
            dbContext = TestDbHelper.CreateInMemoryDbContext();
            dashboardService = new DashboardService(dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private ApplicationUser CreateUser(string id, string userName, string email)
            => new ApplicationUser
            {
                Id = id,
                UserName = userName,
                Email = email,
                FirstName = "Maria",
                LastName = "Tsvetkova",
                DisplayName = "Maria Tsvetkova"
            };

        private Category CreateCategory(string name)
            => new Category
            {
                Id = Guid.NewGuid(),
                Name = name
            };

        private TaskItem CreateTask(string title, string ownerId, Guid categoryId)
            => new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = "Test description",
                Status = WorkStatus.Planned,
                CreatedOn = DateTime.UtcNow,
                OwnerId = ownerId,
                CategoryId = categoryId
            };

        private Note CreateNote(string title, string content, string ownerId)
            => new Note
            {
                Id = Guid.NewGuid(),
                Title = title,
                Content = content,
                CreatedOn = DateTime.UtcNow,
                OwnerId = ownerId
            };

        [Test]
        public async Task GetStatsAsync_ShouldReturnCorrectCounts_ForGivenUser()
        {
            var user1 = CreateUser("user-1", "maria", "maria@test.com");
            var user2 = CreateUser("user-2", "ivan", "ivan@test.com");

            var category1 = CreateCategory("Work");
            var category2 = CreateCategory("Personal");

            await dbContext.Users.AddRangeAsync(user1, user2);
            await dbContext.Categories.AddRangeAsync(category1, category2);

            await dbContext.TaskItems.AddRangeAsync(
                CreateTask("Task 1", user1.Id, category1.Id),
                CreateTask("Task 2", user1.Id, category1.Id),
                CreateTask("Task 3", user2.Id, category2.Id));

            await dbContext.Notes.AddRangeAsync(
                CreateNote("Note 1", "Content 1", user1.Id),
                CreateNote("Note 2", "Content 2", user1.Id),
                CreateNote("Note 3", "Content 3", user2.Id));

            await dbContext.SaveChangesAsync();

            var result = await dashboardService.GetStatsAsync(user1.Id);

            result.MyTasksCount.Should().Be(2);
            result.MyNotesCount.Should().Be(2);
            result.CategoriesCount.Should().Be(2);
            result.TotalUsers.Should().Be(2);
            result.TotalTasks.Should().Be(3);
            result.TotalNotes.Should().Be(3);
        }

        [Test]
        public async Task GetStatsAsync_ShouldReturnZeroCounts_WhenDatabaseIsEmpty()
        {
            var result = await dashboardService.GetStatsAsync("missing-user");

            result.Should().NotBeNull();
            result.MyTasksCount.Should().Be(0);
            result.MyNotesCount.Should().Be(0);
            result.CategoriesCount.Should().Be(0);
            result.TotalUsers.Should().Be(0);
            result.TotalTasks.Should().Be(0);
            result.TotalNotes.Should().Be(0);
        }

        [Test]
        public async Task GetStatsAsync_ShouldReturnOnlyCurrentUserTaskAndNoteCounts()
        {
            var user1 = CreateUser("user-1", "maria", "maria@test.com");
            var user2 = CreateUser("user-2", "ivan", "ivan@test.com");

            var category = CreateCategory("Work");

            await dbContext.Users.AddRangeAsync(user1, user2);
            await dbContext.Categories.AddAsync(category);

            await dbContext.TaskItems.AddRangeAsync(
                CreateTask("User1 Task", user1.Id, category.Id),
                CreateTask("User2 Task 1", user2.Id, category.Id),
                CreateTask("User2 Task 2", user2.Id, category.Id));

            await dbContext.Notes.AddRangeAsync(
                CreateNote("User1 Note", "Content", user1.Id),
                CreateNote("User2 Note 1", "Content", user2.Id),
                CreateNote("User2 Note 2", "Content", user2.Id));

            await dbContext.SaveChangesAsync();

            var result = await dashboardService.GetStatsAsync(user1.Id);

            result.MyTasksCount.Should().Be(1);
            result.MyNotesCount.Should().Be(1);

            result.TotalTasks.Should().Be(3);
            result.TotalNotes.Should().Be(3);
            result.TotalUsers.Should().Be(2);
            result.CategoriesCount.Should().Be(1);
        }
    }
}