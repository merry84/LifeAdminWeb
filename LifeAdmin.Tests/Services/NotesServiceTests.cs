using FluentAssertions;
using LifeAdminData;
using LifeAdminModels.Models;
using LifeAdminServices;

namespace LifeAdmin.Tests.Services
{
    [TestFixture]
    public class NotesServiceTests
    {
        private ApplicationDbContext dbContext;
        private NotesService notesService;

        [SetUp]
        public void SetUp()
        {
            dbContext = TestDbHelper.CreateInMemoryDbContext();
            notesService = new NotesService(dbContext);
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
        public async Task AddAsync_ShouldAddNoteSuccessfully()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            var note = CreateNote("Test Note", "Test Content", user.Id);

            await notesService.AddAsync(note);

            var notesInDb = dbContext.Notes.ToList();

            notesInDb.Should().HaveCount(1);
            notesInDb.First().Title.Should().Be("Test Note");
            notesInDb.First().Content.Should().Be("Test Content");
        }

        [Test]
        public async Task GetMineAsync_ShouldReturnOnlyUserNotesOrderedByCreatedOnDescending()
        {
            var user1 = CreateUser("user-1", "maria", "maria@test.com");
            var user2 = CreateUser("user-2", "ivan", "ivan@test.com");

            await dbContext.Users.AddRangeAsync(user1, user2);

            var olderNote = CreateNote("Older Note", "Old content", user1.Id);
            olderNote.CreatedOn = DateTime.UtcNow.AddDays(-2);

            var newerNote = CreateNote("Newer Note", "New content", user1.Id);
            newerNote.CreatedOn = DateTime.UtcNow;

            var otherUserNote = CreateNote("Ivan Note", "Other content", user2.Id);

            await dbContext.Notes.AddRangeAsync(olderNote, newerNote, otherUserNote);
            await dbContext.SaveChangesAsync();

            var result = (await notesService.GetMineAsync(user1.Id)).ToList();

            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Newer Note");
            result[1].Title.Should().Be("Older Note");
            result.All(n => n.OwnerId == user1.Id).Should().BeTrue();
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNote_WhenNoteExists()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var note = CreateNote("My Note", "Some content", user.Id);
            note.Owner = user;

            await dbContext.Users.AddAsync(user);
            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync();

            var result = await notesService.GetByIdAsync(note.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(note.Id);
            result.Title.Should().Be("My Note");
            result.Owner.Should().NotBeNull();
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNoteDoesNotExist()
        {
            var result = await notesService.GetByIdAsync(Guid.NewGuid());

            result.Should().BeNull();
        }

        [Test]
        public async Task GetByIdOwnedAsync_ShouldReturnNote_WhenOwnedByUser()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var note = CreateNote("Owned Note", "Owned content", user.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync();

            var result = await notesService.GetByIdOwnedAsync(note.Id, user.Id);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Owned Note");
        }

        [Test]
        public async Task GetByIdOwnedAsync_ShouldReturnNull_WhenNoteIsNotOwnedByUser()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var note = CreateNote("Private Note", "Private content", user.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync();

            var result = await notesService.GetByIdOwnedAsync(note.Id, "user-2");

            result.Should().BeNull();
        }

        [Test]
        public async Task UpdateAsync_ShouldUpdateNoteSuccessfully()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var note = CreateNote("Old Title", "Old content", user.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync();

            note.Title = "New Title";
            note.Content = "New content";

            await notesService.UpdateAsync(note);

            var updatedNote = await dbContext.Notes.FindAsync(note.Id);

            updatedNote.Should().NotBeNull();
            updatedNote!.Title.Should().Be("New Title");
            updatedNote.Content.Should().Be("New content");
        }

        [Test]
        public async Task DeleteAsync_ShouldSoftDeleteNote()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var note = CreateNote("Note To Delete", "Delete me", user.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync();

            await notesService.DeleteAsync(note);

            note.IsDeleted.Should().BeTrue();
            note.DeletedOn.Should().NotBeNull();
        }

        [Test]
        public async Task RestoreAsync_ShouldRestoreDeletedNote()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var note = CreateNote("Deleted Note", "Restore me", user.Id);
            note.IsDeleted = true;
            note.DeletedOn = DateTime.UtcNow;

            await dbContext.Users.AddAsync(user);
            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync();

            await notesService.RestoreAsync(note);

            note.IsDeleted.Should().BeFalse();
            note.DeletedOn.Should().BeNull();
        }

        [Test]
        public async Task GetDeletedAsync_ShouldReturnOnlyDeletedNotesOrderedByDeletedOnDescending()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            await dbContext.Users.AddAsync(user);

            var olderDeleted = CreateNote("Older Deleted", "old", user.Id);
            olderDeleted.IsDeleted = true;
            olderDeleted.DeletedOn = DateTime.UtcNow.AddDays(-2);

            var newerDeleted = CreateNote("Newer Deleted", "new", user.Id);
            newerDeleted.IsDeleted = true;
            newerDeleted.DeletedOn = DateTime.UtcNow;

            var activeNote = CreateNote("Active Note", "active", user.Id);

            await dbContext.Notes.AddRangeAsync(olderDeleted, newerDeleted, activeNote);
            await dbContext.SaveChangesAsync();

            var result = (await notesService.GetDeletedAsync()).ToList();

            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Newer Deleted");
            result[1].Title.Should().Be("Older Deleted");
            result.All(n => n.IsDeleted).Should().BeTrue();
        }

        [Test]
        public async Task GetDeletedByIdAsync_ShouldReturnDeletedNote_WhenExists()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var note = CreateNote("Deleted Note", "Deleted content", user.Id);
            note.IsDeleted = true;
            note.DeletedOn = DateTime.UtcNow;

            await dbContext.Users.AddAsync(user);
            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync();

            var result = await notesService.GetDeletedByIdAsync(note.Id);

            result.Should().NotBeNull();
            result!.Id.Should().Be(note.Id);
            result.IsDeleted.Should().BeTrue();
        }

        [Test]
        public async Task GetDeletedByIdAsync_ShouldReturnNull_WhenNoteIsNotDeleted()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            var note = CreateNote("Active Note", "Active content", user.Id);

            await dbContext.Users.AddAsync(user);
            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync();

            var result = await notesService.GetDeletedByIdAsync(note.Id);

            result.Should().BeNull();
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnPagedNotesForUser()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            await dbContext.Users.AddAsync(user);

            var note1 = CreateNote("Note 1", "Content 1", user.Id);
            note1.CreatedOn = DateTime.UtcNow.AddDays(-3);

            var note2 = CreateNote("Note 2", "Content 2", user.Id);
            note2.CreatedOn = DateTime.UtcNow.AddDays(-2);

            var note3 = CreateNote("Note 3", "Content 3", user.Id);
            note3.CreatedOn = DateTime.UtcNow.AddDays(-1);

            await dbContext.Notes.AddRangeAsync(note1, note2, note3);
            await dbContext.SaveChangesAsync();

            var result = await notesService.GetAllAsync(user.Id, null, 1, 2);

            result.TotalNotesCount.Should().Be(3);
            result.Notes.Should().HaveCount(2);
            result.Notes.First().Title.Should().Be("Note 3");
            result.Notes.Last().Title.Should().Be("Note 2");
            result.CurrentPage.Should().Be(1);
            result.NotesPerPage.Should().Be(2);
        }

        [Test]
        public async Task GetAllAsync_ShouldFilterNotesBySearchTermInTitle()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            await dbContext.Users.AddAsync(user);

            var matchingNote = CreateNote("Shopping List", "Buy milk", user.Id);
            var nonMatchingNote = CreateNote("Workout Plan", "Run 5km", user.Id);

            await dbContext.Notes.AddRangeAsync(matchingNote, nonMatchingNote);
            await dbContext.SaveChangesAsync();

            var result = await notesService.GetAllAsync(user.Id, "shop", 1, 10);

            result.TotalNotesCount.Should().Be(1);
            result.Notes.Should().HaveCount(1);
            result.Notes.First().Title.Should().Be("Shopping List");
        }

        [Test]
        public async Task GetAllAsync_ShouldFilterNotesBySearchTermInContent()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            await dbContext.Users.AddAsync(user);

            var matchingNote = CreateNote("Random Title", "Need to buy milk today", user.Id);
            var nonMatchingNote = CreateNote("Workout", "Run and stretch", user.Id);

            await dbContext.Notes.AddRangeAsync(matchingNote, nonMatchingNote);
            await dbContext.SaveChangesAsync();

            var result = await notesService.GetAllAsync(user.Id, "milk", 1, 10);

            result.TotalNotesCount.Should().Be(1);
            result.Notes.Should().HaveCount(1);
            result.Notes.First().Title.Should().Be("Random Title");
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnContentPreviewWithEllipsis_WhenContentIsLongerThan120Characters()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            await dbContext.Users.AddAsync(user);

            var longContent = new string('A', 130);
            var note = CreateNote("Long Note", longContent, user.Id);

            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync();

            var result = await notesService.GetAllAsync(user.Id, null, 1, 10);

            result.Notes.Should().HaveCount(1);
            result.Notes.First().ContentPreview.Should().HaveLength(123);
            result.Notes.First().ContentPreview.Should().EndWith("...");
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnFullContent_WhenContentIs120CharactersOrLess()
        {
            var user = CreateUser("user-1", "maria", "maria@test.com");
            await dbContext.Users.AddAsync(user);

            var shortContent = new string('B', 120);
            var note = CreateNote("Short Note", shortContent, user.Id);

            await dbContext.Notes.AddAsync(note);
            await dbContext.SaveChangesAsync();

            var result = await notesService.GetAllAsync(user.Id, null, 1, 10);

            result.Notes.Should().HaveCount(1);
            result.Notes.First().ContentPreview.Should().Be(shortContent);
        }
    }
}