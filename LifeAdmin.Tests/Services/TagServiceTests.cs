using FluentAssertions;
using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdmin.Tests.Helpers;
using NUnit.Framework;

namespace LifeAdmin.Tests.Services
{
    [TestFixture]
    public class TagServiceTests
    {
        private ApplicationDbContext dbContext;
        private TagService tagService;

        [SetUp]
        public void SetUp()
        {
            dbContext = TestDbHelper.CreateInMemoryDbContext();
            tagService = new TagService(dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllTags()
        {
            var firstTag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = "Work"
            };

            var secondTag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = "Personal"
            };

            await dbContext.Tags.AddRangeAsync(firstTag, secondTag);
            await dbContext.SaveChangesAsync();

            var result = (await tagService.GetAllAsync()).ToList();

            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Name == "Work");
            result.Should().Contain(t => t.Name == "Personal");
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnEmptyCollection_WhenNoTagsExist()
        {
            var result = (await tagService.GetAllAsync()).ToList();

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnTagsFromDatabase()
        {
            var tag = new Tag
            {
                Id = Guid.NewGuid(),
                Name = "Important"
            };

            await dbContext.Tags.AddAsync(tag);
            await dbContext.SaveChangesAsync();

            var result = await tagService.GetAllAsync();

            result.Should().ContainSingle();
            result.First().Name.Should().Be("Important");
        }
    }
}