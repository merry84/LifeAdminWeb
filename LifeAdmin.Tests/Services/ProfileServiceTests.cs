using FluentAssertions;
using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices;
using NUnit.Framework;
using ViewModels.ProfilesViewModels;

namespace LifeAdmin.Tests.Services
{
    [TestFixture]
    public class ProfileServiceTests
    {
        private ApplicationDbContext dbContext;
        private ProfileService profileService;

        [SetUp]
        public void SetUp()
        {
            dbContext = TestDbHelper.CreateInMemoryDbContext();
            profileService = new ProfileService(dbContext);
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
                DisplayName = "Maria Tsvetkova",
                ProfileImageUrl = "https://example.com/profile.jpg",
                Bio = "Test bio",
                PhoneNumber = "0888000000",
                CreatedOn = DateTime.UtcNow
            };

        [Test]
        public async Task GetProfileAsync_ShouldReturnProfile_WhenUserExists()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var result = await profileService.GetProfileAsync(user.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id);
            result.UserName.Should().Be("maria");
            result.Email.Should().Be("maria@test.com");
            result.DisplayName.Should().Be("Maria Tsvetkova");
            result.FirstName.Should().Be("Maria");
            result.LastName.Should().Be("Tsvetkova");
            result.ProfileImageUrl.Should().Be("https://example.com/profile.jpg");
            result.Bio.Should().Be("Test bio");
            result.PhoneNumber.Should().Be("0888000000");
        }

        [Test]
        public async Task GetProfileAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var result = await profileService.GetProfileAsync("missing-user");

            result.Should().BeNull();
        }

        [Test]
        public async Task GetProfileForEditAsync_ShouldReturnEditModel_WhenUserExists()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var result = await profileService.GetProfileForEditAsync(user.Id);

            result.Should().NotBeNull();
            result!.DisplayName.Should().Be("Maria Tsvetkova");
            result.FirstName.Should().Be("Maria");
            result.LastName.Should().Be("Tsvetkova");
            result.ProfileImageUrl.Should().Be("https://example.com/profile.jpg");
            result.CurrentProfileImageUrl.Should().Be("https://example.com/profile.jpg");
            result.Bio.Should().Be("Test bio");
            result.PhoneNumber.Should().Be("0888000000");
        }

        [Test]
        public async Task GetProfileForEditAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            var result = await profileService.GetProfileForEditAsync("missing-user");

            result.Should().BeNull();
        }

        [Test]
        public async Task UpdateProfileAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            var model = new EditProfileViewModel
            {
                DisplayName = "Updated Name",
                FirstName = "Updated",
                LastName = "User",
                ProfileImageUrl = "https://example.com/new.jpg",
                Bio = "Updated bio",
                PhoneNumber = "0999000000",
                CurrentProfileImageUrl = "https://example.com/current.jpg"
            };

            var result = await profileService.UpdateProfileAsync("missing-user", model);

            result.Should().BeFalse();
        }

        [Test]
        public async Task UpdateProfileAsync_ShouldUpdateUserAndReturnTrue_WhenUserExists()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var model = new EditProfileViewModel
            {
                DisplayName = "Updated Display Name",
                FirstName = "Updated First",
                LastName = "Updated Last",
                ProfileImageUrl = "https://example.com/new-image.jpg",
                Bio = "Updated bio",
                PhoneNumber = "0999111222",
                CurrentProfileImageUrl = ""
            };

            var result = await profileService.UpdateProfileAsync(user.Id, model);
            var updatedUser = await dbContext.Users.FindAsync(user.Id);

            result.Should().BeTrue();
            updatedUser.Should().NotBeNull();
            updatedUser!.DisplayName.Should().Be("Updated Display Name");
            updatedUser.FirstName.Should().Be("Updated First");
            updatedUser.LastName.Should().Be("Updated Last");
            updatedUser.ProfileImageUrl.Should().Be("https://example.com/new-image.jpg");
            updatedUser.Bio.Should().Be("Updated bio");
            updatedUser.PhoneNumber.Should().Be("0999111222");
        }

        [Test]
        public async Task UpdateProfileAsync_ShouldUseCurrentProfileImageUrl_WhenProvided()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");

            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var model = new EditProfileViewModel
            {
                DisplayName = "Updated Display Name",
                FirstName = "Updated First",
                LastName = "Updated Last",
                ProfileImageUrl = "https://example.com/new-image.jpg",
                CurrentProfileImageUrl = "https://example.com/current-image.jpg",
                Bio = "Updated bio",
                PhoneNumber = "0999111222"
            };

            var result = await profileService.UpdateProfileAsync(user.Id, model);
            var updatedUser = await dbContext.Users.FindAsync(user.Id);

            result.Should().BeTrue();
            updatedUser.Should().NotBeNull();
            updatedUser!.ProfileImageUrl.Should().Be("https://example.com/current-image.jpg");
        }
    }
}