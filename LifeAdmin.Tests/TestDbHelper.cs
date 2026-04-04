using LifeAdminData;
using Microsoft.EntityFrameworkCore;

namespace LifeAdmin.Tests
{
    public static class TestDbHelper
    {
        public static ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}