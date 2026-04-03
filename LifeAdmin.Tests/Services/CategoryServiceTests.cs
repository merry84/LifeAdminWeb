using FluentAssertions;
using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices;
using LifeAdmin.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using NUnit.Framework;

namespace LifeAdmin.Tests.Services
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private ApplicationDbContext dbContext;
        private CategoryService categoryService;

        [SetUp]
        public void SetUp()
        {
            dbContext = TestDbHelper.CreateInMemoryDbContext();
            categoryService = new CategoryService(dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task AddAsync_ShouldAddCategoryToDatabase()
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Work"
            };

            await categoryService.AddAsync(category);

            var categoriesInDb = dbContext.Categories.ToList();

            categoriesInDb.Should().HaveCount(1);
            categoriesInDb.First().Name.Should().Be("Work");
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnTrue_WhenCategoryExists()
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Personal"
            };

            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            var result = await categoryService.ExistsAsync(category.Id);

            result.Should().BeTrue();
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalse_WhenCategoryDoesNotExist()
        {
            var missingId = Guid.NewGuid();

            var result = await categoryService.ExistsAsync(missingId);

            result.Should().BeFalse();
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllCategoriesOrderedByName()
        {
            await dbContext.Categories.AddRangeAsync(
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Work"
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Health"
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Personal"
                });
            await dbContext.SaveChangesAsync();

            var result = (await categoryService.GetAllAsync()).ToList();

            result.Should().HaveCount(3);
            result[0].Name.Should().Be("Health");
            result[1].Name.Should().Be("Personal");
            result[2].Name.Should().Be("Work");
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCategory_WhenIdExists()
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Fitness"
            };

            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            var result = await categoryService.GetByIdAsync(category.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(category.Id);
            result.Name.Should().Be("Fitness");
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenIdDoesNotExist()
        {
            var result = await categoryService.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateCategorySuccessfully()
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Old Name"
            };

            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            category.Name = "New Name";

            await categoryService.UpdateAsync(category);

            var updatedCategory = await dbContext.Categories.FindAsync(category.Id);

            updatedCategory.Should().NotBeNull();
            updatedCategory!.Name.Should().Be("New Name");
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveCategoryFromDatabase()
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "To Delete"
            };

            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            await categoryService.DeleteAsync(category);

            var categoriesInDb = dbContext.Categories.ToList();

            categoriesInDb.Should().BeEmpty();
        }

        [Test]
        public async Task GetAllForSelectAsync_ShouldReturnAllCategoriesAsSelectListItemsOrderedByName()
        {
            var firstCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Work"
            };

            var secondCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Health"
            };

            await dbContext.Categories.AddRangeAsync(firstCategory, secondCategory);
            await dbContext.SaveChangesAsync();

            var result = (await categoryService.GetAllForSelectAsync()).ToList();

            result.Should().HaveCount(2);
            result[0].Should().BeOfType<SelectListItem>();
            result[0].Text.Should().Be("Health");
            result[0].Value.Should().Be(secondCategory.Id.ToString());
            result[1].Text.Should().Be("Work");
            result[1].Value.Should().Be(firstCategory.Id.ToString());
        }
    }
}