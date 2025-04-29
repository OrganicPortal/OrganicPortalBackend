using Microsoft.EntityFrameworkCore;
using OrganicPortalBackend.Models.Database.RegUser;
using OrganicPortalBackend.Models.Database.User;
using OrganicPortalBackend.Models.Database.User.Recovery;
using OrganicPortalBackend.Models.Database.User.Session;

namespace OrganicPortalBackend.Models.Database
{
    public class OrganicContext : DbContext
    {
        public OrganicContext(DbContextOptions<OrganicContext> options) : base(options) { }


        // Таблиці користувача
        /* */
        public DbSet<RegModel> RegTable { get; set; }

        public DbSet<UserModel> UserTable { get; set; }
        public DbSet<PasswordModel> PasswordTable { get; set; }
        public DbSet<PhoneModel> PhoneTable { get; set; }

        public DbSet<SessionModel> SessionTable { get; set; }

        public DbSet<RecoveryModel> RecoveryTable { get; set; }
        /* */



    }
}