using FluentAssertions;
using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices;
using LifeAdmin.Tests.Helpers;
using NUnit.Framework;

namespace LifeAdmin.Tests.Services
{
    [TestFixture]
    public class TaskServiceTests
    {
        private ApplicationDbContext dbContext;
        private TaskService taskService;

        [SetUp]
        public void SetUp()
        {
            dbContext = TestDbHelper.CreateInMemoryDbContext();
            taskService = new TaskService(dbContext);
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
         LastName = "Tsvetkova"
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

        [Test]
        public async Task AddAsync_ShouldAddTaskSuccessfully()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Work");

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            var task = CreateTask("Test Task", user.Id, category.Id);

            await taskService.AddAsync(task);

            var tasksInDb = dbContext.TaskItems.ToList();

            tasksInDb.Should().HaveCount(1);
            tasksInDb.First().Title.Should().Be("Test Task");
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Personal");

            var task = CreateTask("My Task", user.Id, category.Id);
            task.Owner = user;
            task.Category = category;

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);
            await dbContext.TaskItems.AddAsync(task);
            await dbContext.SaveChangesAsync();

            var result = await taskService.GetByIdAsync(task.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(task.Id);
            result.Title.Should().Be("My Task");
            result.Category.Should().NotBeNull();
            result.Owner.Should().NotBeNull();
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            var result = await taskService.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllTasksOrderedByCreatedOnDescending()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Work");

            var olderTask = CreateTask("Older Task", user.Id, category.Id);
            olderTask.CreatedOn = DateTime.UtcNow.AddDays(-2);
            olderTask.Owner = user;
            olderTask.Category = category;

            var newerTask = CreateTask("Newer Task", user.Id, category.Id);
            newerTask.CreatedOn = DateTime.UtcNow;
            newerTask.Owner = user;
            newerTask.Category = category;

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);
            await dbContext.TaskItems.AddRangeAsync(olderTask, newerTask);
            await dbContext.SaveChangesAsync();

            var result = (await taskService.GetAllAsync()).ToList();

            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Newer Task");
            result[1].Title.Should().Be("Older Task");
        }

        [Test]
        public async Task GetAllAsync_WithUserId_ShouldReturnOnlyTasksForGivenUser()
        {
            var user1 = CreateUser("user-1", "maria", "maria@test.com");
            var user2 = CreateUser("user-2", "ivan", "ivan@test.com");
            var category = CreateCategory("Home");

            await dbContext.Users.AddRangeAsync(user1, user2);
            await dbContext.Categories.AddAsync(category);

            var task1 = CreateTask("Task for Maria", user1.Id, category.Id);
            var task2 = CreateTask("Task for Ivan", user2.Id, category.Id);

            await dbContext.TaskItems.AddRangeAsync(task1, task2);
            await dbContext.SaveChangesAsync();

            var result = (await taskService.GetAllAsync(user1.Id)).ToList();

            result.Should().HaveCount(1);
            result[0].Title.Should().Be("Task for Maria");
            result[0].OwnerId.Should().Be(user1.Id);
        }

        [Test]
        public async Task GetMineAsync_ShouldReturnOnlyTasksForGivenUser()
        {
            var user1 = CreateUser("user-1", "maria", "maria@test.com");
            var user2 = CreateUser("user-2", "ivan", "ivan@test.com");
            var category = CreateCategory("Study");

            await dbContext.Users.AddRangeAsync(user1, user2);
            await dbContext.Categories.AddAsync(category);

            var task1 = CreateTask("Maria Task", user1.Id, category.Id);
            var task2 = CreateTask("Ivan Task", user2.Id, category.Id);

            await dbContext.TaskItems.AddRangeAsync(task1, task2);
            await dbContext.SaveChangesAsync();

            var result = (await taskService.GetMineAsync(user1.Id)).ToList();

            result.Should().HaveCount(1);
            result[0].Title.Should().Be("Maria Task");
        }

        [Test]
        public async Task GetByIdOwnedAsync_ShouldReturnTask_WhenTaskBelongsToUser()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Errands");
            var task = CreateTask("Owned Task", user.Id, category.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);
            await dbContext.TaskItems.AddAsync(task);
            await dbContext.SaveChangesAsync();

            var result = await taskService.GetByIdOwnedAsync(task.Id, user.Id);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Owned Task");
        }

        [Test]
        public async Task GetByIdOwnedAsync_ShouldReturnNull_WhenTaskDoesNotBelongToUser()
        {
            var owner = CreateUser("user-1", "owner", "owner@test.com");
            var category = CreateCategory("Work");
            var task = CreateTask("Secret Task", owner.Id, category.Id);

            await dbContext.Users.AddAsync(owner);
            await dbContext.Categories.AddAsync(category);
            await dbContext.TaskItems.AddAsync(task);
            await dbContext.SaveChangesAsync();

            var result = await taskService.GetByIdOwnedAsync(task.Id, "user-2");

            result.Should().BeNull();
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateTaskSuccessfully()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Work");
            var task = CreateTask("Old Title", user.Id, category.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);
            await dbContext.TaskItems.AddAsync(task);
            await dbContext.SaveChangesAsync();

            task.Title = "New Title";

            await taskService.UpdateAsync(task);

            var updatedTask = await dbContext.TaskItems.FindAsync(task.Id);

            updatedTask.Should().NotBeNull();
            updatedTask!.Title.Should().Be("New Title");
        }

        [Test]
        public async Task DeleteAsync_ShouldSoftDeleteTask()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Work");
            var task = CreateTask("Task To Delete", user.Id, category.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);
            await dbContext.TaskItems.AddAsync(task);
            await dbContext.SaveChangesAsync();

            await taskService.DeleteAsync(task);

            task.IsDeleted.Should().BeTrue();
            task.DeletedOn.Should().NotBeNull();
        }

        [Test]
        public async Task RestoreAsync_ShouldRestoreSoftDeletedTask()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Work");
            var task = CreateTask("Deleted Task", user.Id, category.Id);

            task.IsDeleted = true;
            task.DeletedOn = DateTime.UtcNow;

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);
            await dbContext.TaskItems.AddAsync(task);
            await dbContext.SaveChangesAsync();

            await taskService.RestoreAsync(task);

            task.IsDeleted.Should().BeFalse();
            task.DeletedOn.Should().BeNull();
        }

        [Test]
        public async Task ExistsOwnedAsync_ShouldReturnTrue_WhenTaskBelongsToUser()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Study");
            var task = CreateTask("Owned Task", user.Id, category.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);
            await dbContext.TaskItems.AddAsync(task);
            await dbContext.SaveChangesAsync();

            var result = await taskService.ExistsOwnedAsync(task.Id, user.Id);

            result.Should().BeTrue();
        }

        [Test]
        public async Task ExistsOwnedAsync_ShouldReturnFalse_WhenTaskDoesNotBelongToUser()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Study");
            var task = CreateTask("Owned Task", user.Id, category.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);
            await dbContext.TaskItems.AddAsync(task);
            await dbContext.SaveChangesAsync();

            var result = await taskService.ExistsOwnedAsync(task.Id, "another-user");

            result.Should().BeFalse();
        }
        [Test]
        public async Task GetDeletedAsync_ShouldReturnOnlyDeletedTasksOrderedByDeletedOnDescending()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Work");

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);

            var olderDeletedTask = CreateTask("Older Deleted", user.Id, category.Id);
            olderDeletedTask.IsDeleted = true;
            olderDeletedTask.DeletedOn = DateTime.UtcNow.AddDays(-2);
            olderDeletedTask.Owner = user;
            olderDeletedTask.Category = category;

            var newerDeletedTask = CreateTask("Newer Deleted", user.Id, category.Id);
            newerDeletedTask.IsDeleted = true;
            newerDeletedTask.DeletedOn = DateTime.UtcNow;
            newerDeletedTask.Owner = user;
            newerDeletedTask.Category = category;

            var activeTask = CreateTask("Active Task", user.Id, category.Id);

            await dbContext.TaskItems.AddRangeAsync(olderDeletedTask, newerDeletedTask, activeTask);
            await dbContext.SaveChangesAsync();

            var result = (await taskService.GetDeletedAsync()).ToList();

            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Newer Deleted");
            result[1].Title.Should().Be("Older Deleted");
            result.All(t => t.IsDeleted).Should().BeTrue();
        }

        [Test]
        public async Task GetDeletedByIdAsync_ShouldReturnDeletedTask_WhenExists()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Work");
            var task = CreateTask("Deleted Task", user.Id, category.Id);

            task.IsDeleted = true;
            task.DeletedOn = DateTime.UtcNow;

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);
            await dbContext.TaskItems.AddAsync(task);
            await dbContext.SaveChangesAsync();

            var result = await taskService.GetDeletedByIdAsync(task.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(task.Id);
            result.IsDeleted.Should().BeTrue();
        }

        [Test]
        public async Task GetDeletedByIdAsync_ShouldReturnNull_WhenTaskIsNotDeleted()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Work");
            var task = CreateTask("Active Task", user.Id, category.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);
            await dbContext.TaskItems.AddAsync(task);
            await dbContext.SaveChangesAsync();

            var result = await taskService.GetDeletedByIdAsync(task.Id);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetAllAsync_Query_ShouldReturnPagedTasksForUser()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Work");

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);

            var task1 = CreateTask("Task 1", user.Id, category.Id);
            task1.CreatedOn = DateTime.UtcNow.AddDays(-3);
            task1.Owner = user;
            task1.Category = category;

            var task2 = CreateTask("Task 2", user.Id, category.Id);
            task2.CreatedOn = DateTime.UtcNow.AddDays(-2);
            task2.Owner = user;
            task2.Category = category;

            var task3 = CreateTask("Task 3", user.Id, category.Id);
            task3.CreatedOn = DateTime.UtcNow.AddDays(-1);
            task3.Owner = user;
            task3.Category = category;

            await dbContext.TaskItems.AddRangeAsync(task1, task2, task3);
            await dbContext.SaveChangesAsync();

            var result = await taskService.GetAllAsync(user.Id, null, null, null, 1, 2);

            result.TotalTasksCount.Should().Be(3);
            result.Tasks.Should().HaveCount(2);
            result.Tasks.First().Title.Should().Be("Task 3");
            result.Tasks.Last().Title.Should().Be("Task 2");
            result.CurrentPage.Should().Be(1);
            result.TasksPerPage.Should().Be(2);
            result.Categories.Should().HaveCount(1);
        }

        [Test]
        public async Task GetAllAsync_Query_ShouldFilterBySearchTerm()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Work");

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);

            var matchingTask = CreateTask("Shopping Task", user.Id, category.Id);
            matchingTask.Owner = user;
            matchingTask.Category = category;

            var nonMatchingTask = CreateTask("Workout Task", user.Id, category.Id);
            nonMatchingTask.Owner = user;
            nonMatchingTask.Category = category;

            await dbContext.TaskItems.AddRangeAsync(matchingTask, nonMatchingTask);
            await dbContext.SaveChangesAsync();

            var result = await taskService.GetAllAsync(user.Id, "shop", null, null, 1, 10);

            result.TotalTasksCount.Should().Be(1);
            result.Tasks.Should().HaveCount(1);
            result.Tasks.First().Title.Should().Be("Shopping Task");
        }

        [Test]
        public async Task GetAllAsync_Query_ShouldFilterByCategory()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var workCategory = CreateCategory("Work");
            var personalCategory = CreateCategory("Personal");

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddRangeAsync(workCategory, personalCategory);

            var workTask = CreateTask("Work Task", user.Id, workCategory.Id);
            workTask.Owner = user;
            workTask.Category = workCategory;

            var personalTask = CreateTask("Personal Task", user.Id, personalCategory.Id);
            personalTask.Owner = user;
            personalTask.Category = personalCategory;

            await dbContext.TaskItems.AddRangeAsync(workTask, personalTask);
            await dbContext.SaveChangesAsync();

            var result = await taskService.GetAllAsync(user.Id, null, workCategory.Id, null, 1, 10);

            result.TotalTasksCount.Should().Be(1);
            result.Tasks.Should().HaveCount(1);
            result.Tasks.First().Title.Should().Be("Work Task");
            result.Tasks.First().CategoryName.Should().Be("Work");
        }

        [Test]
        public async Task GetAllAsync_Query_ShouldFilterByStatus()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var category = CreateCategory("Work");

            await dbContext.Users.AddAsync(user);
            await dbContext.Categories.AddAsync(category);

            var plannedTask = CreateTask("Planned Task", user.Id, category.Id);
            plannedTask.Status = WorkStatus.Planned;
            plannedTask.Owner = user;
            plannedTask.Category = category;

            var doneTask = CreateTask("Done Task", user.Id, category.Id);
            doneTask.Status = WorkStatus.Done;
            doneTask.Owner = user;
            doneTask.Category = category;

            await dbContext.TaskItems.AddRangeAsync(plannedTask, doneTask);
            await dbContext.SaveChangesAsync();

            var result = await taskService.GetAllAsync(user.Id, null, null, (int)WorkStatus.Done, 1, 10);

            result.TotalTasksCount.Should().Be(1);
            result.Tasks.Should().HaveCount(1);
            result.Tasks.First().Title.Should().Be("Done Task");
        }
    }
}