using FluentAssertions;
using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace LifeAdmin.Tests.Services
{
    [TestFixture]
    public class DocumentServiceTests
    {
        private ApplicationDbContext dbContext;
        private DocumentService documentService;

        [SetUp]
        public void SetUp()
        {
            dbContext = TestDbHelper.CreateInMemoryDbContext();
            documentService = new DocumentService(dbContext);
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

        private Document CreateDocument(string title, string ownerId)
            => new Document
             {
                 Id = Guid.NewGuid(),
                 Title = title,
                 FileName = $"{title}.pdf",
                 StoredFileName = $"{Guid.NewGuid()}.pdf",
                 ContentType = "application/pdf",
                 FileSize = 1024,
                 UploadedOn = DateTime.UtcNow,
                 OwnerId = ownerId
             };

        [Test]
        public async Task AddAsync_ShouldAddDocumentSuccessfully()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var document = CreateDocument("Test Document", user.Id);

            await documentService.AddAsync(document);

            var documentsInDb = dbContext.Documents.ToList();

            documentsInDb.Should().HaveCount(1);
            documentsInDb.First().Title.Should().Be("Test Document");
        }

        [Test]
        public async Task GetMineAsync_ShouldReturnOnlyDocumentsForGivenUserOrderedByUploadedOnDescending()
        {
            var user1 = CreateUser("user-1", "maria", "maria@test.com");
            var user2 = CreateUser("user-2", "ivan", "ivan@test.com");

            await dbContext.Users.AddRangeAsync(user1, user2);

            var olderDoc = CreateDocument("Older Doc", user1.Id);
            olderDoc.UploadedOn = DateTime.UtcNow.AddDays(-2);

            var newerDoc = CreateDocument("Newer Doc", user1.Id);
            newerDoc.UploadedOn = DateTime.UtcNow;

            var otherUserDoc = CreateDocument("Ivan Doc", user2.Id);

            await dbContext.Documents.AddRangeAsync(olderDoc, newerDoc, otherUserDoc);
            await dbContext.SaveChangesAsync();

            var result = (await documentService.GetMineAsync(user1.Id)).ToList();

            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Newer Doc");
            result[1].Title.Should().Be("Older Doc");
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllDocumentsOrderedByUploadedOnDescending()
        {
            var user1 = CreateUser("user-1", "maria", "maria@test.com");
            var user2 = CreateUser("user-2", "ivan", "ivan@test.com");

            await dbContext.Users.AddRangeAsync(user1, user2);

            var olderDoc = CreateDocument("Older Doc", user1.Id);
            olderDoc.UploadedOn = DateTime.UtcNow.AddDays(-2);
            olderDoc.Owner = user1;

            var newerDoc = CreateDocument("Newer Doc", user2.Id);
            newerDoc.UploadedOn = DateTime.UtcNow;
            newerDoc.Owner = user2;

            await dbContext.Documents.AddRangeAsync(olderDoc, newerDoc);
            await dbContext.SaveChangesAsync();

            var result = (await documentService.GetAllAsync()).ToList();

            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Newer Doc");
            result[0].OwnerUserName.Should().Be("ivan");
            result[1].Title.Should().Be("Older Doc");
            result[1].OwnerUserName.Should().Be("maria");
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnDocument_WhenExists()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var document = CreateDocument("My Doc", user.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Documents.AddAsync(document);
            await dbContext.SaveChangesAsync();

            var result = await documentService.GetByIdAsync(document.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(document.Id);
            result.Title.Should().Be("My Doc");
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenDocumentDoesNotExist()
        {
            var result = await documentService.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Test]
        public async Task GetByIdOwnedAsync_ShouldReturnDocument_WhenOwnedByUser()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var document = CreateDocument("Owned Doc", user.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Documents.AddAsync(document);
            await dbContext.SaveChangesAsync();

            var result = await documentService.GetByIdOwnedAsync(document.Id, user.Id);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Owned Doc");
        }

        [Test]
        public async Task GetByIdOwnedAsync_ShouldReturnNull_WhenDocumentIsNotOwnedByUser()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var document = CreateDocument("Private Doc", user.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Documents.AddAsync(document);
            await dbContext.SaveChangesAsync();

            var result = await documentService.GetByIdOwnedAsync(document.Id, "user-2");

            result.Should().BeNull();
        }

        [Test]
        public async Task DeleteAsync_ShouldSoftDeleteDocument()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var document = CreateDocument("Delete Me", user.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Documents.AddAsync(document);
            await dbContext.SaveChangesAsync();

            await documentService.DeleteAsync(document);

            var updatedDocument = await dbContext.Documents
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == document.Id);

            updatedDocument.Should().NotBeNull();
            updatedDocument!.IsDeleted.Should().BeTrue();
            updatedDocument.DeletedOn.Should().NotBeNull();
        }

        [Test]
        public async Task RestoreAsync_ShouldRestoreDeletedDocument()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var document = CreateDocument("Deleted Doc", user.Id);
            document.IsDeleted = true;
            document.DeletedOn = DateTime.UtcNow;

            await dbContext.Users.AddAsync(user);
            await dbContext.Documents.AddAsync(document);
            await dbContext.SaveChangesAsync();

            await documentService.RestoreAsync(document);

            var updatedDocument = await dbContext.Documents
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(d => d.Id == document.Id);

            updatedDocument.Should().NotBeNull();
            updatedDocument!.IsDeleted.Should().BeFalse();
            updatedDocument.DeletedOn.Should().BeNull();
        }

        [Test]
        public async Task GetDeletedAsync_ShouldReturnOnlyDeletedDocumentsOrderedByDeletedOnDescending()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            await dbContext.Users.AddAsync(user);

            var olderDeleted = CreateDocument("Older Deleted", user.Id);
            olderDeleted.IsDeleted = true;
            olderDeleted.DeletedOn = DateTime.UtcNow.AddDays(-2);
            olderDeleted.Owner = user;

            var newerDeleted = CreateDocument("Newer Deleted", user.Id);
            newerDeleted.IsDeleted = true;
            newerDeleted.DeletedOn = DateTime.UtcNow;
            newerDeleted.Owner = user;

            var activeDoc = CreateDocument("Active Doc", user.Id);

            await dbContext.Documents.AddRangeAsync(olderDeleted, newerDeleted, activeDoc);
            await dbContext.SaveChangesAsync();

            var result = (await documentService.GetDeletedAsync()).ToList();

            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Newer Deleted");
            result[1].Title.Should().Be("Older Deleted");
            result.All(d => d.OwnerUserName == "maria").Should().BeTrue();
        }

        [Test]
        public async Task GetDeletedByIdAsync_ShouldReturnDeletedDocument_WhenExists()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var document = CreateDocument("Deleted Doc", user.Id);
            document.IsDeleted = true;
            document.DeletedOn = DateTime.UtcNow;

            await dbContext.Users.AddAsync(user);
            await dbContext.Documents.AddAsync(document);
            await dbContext.SaveChangesAsync();

            var result = await documentService.GetDeletedByIdAsync(document.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(document.Id);
            result.IsDeleted.Should().BeTrue();
        }

        [Test]
        public async Task GetDeletedByIdAsync_ShouldReturnNull_WhenDocumentIsNotDeleted()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var document = CreateDocument("Active Doc", user.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Documents.AddAsync(document);
            await dbContext.SaveChangesAsync();

            var result = await documentService.GetDeletedByIdAsync(document.Id);

            result.Should().BeNull();
        }
    }
}