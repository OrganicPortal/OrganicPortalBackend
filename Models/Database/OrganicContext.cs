using Microsoft.EntityFrameworkCore;
using OrganicPortalBackend.Models.Database.Company;
using OrganicPortalBackend.Models.Database.RegUser;
using OrganicPortalBackend.Models.Database.Seed;
using OrganicPortalBackend.Models.Database.Seed.CERT;
using OrganicPortalBackend.Models.Database.Solana;
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



        // Таблиці компанії
        /* */
        public DbSet<CompanyModel> CompanyTable { get; set; }
        public DbSet<CompanyContactModel> CompanyContactTable { get; set; }
        public DbSet<CompanyTypeOfActivityModel> CompanyTypeOfActivityTable { get; set; }
        public DbSet<EmployeesModel> EmployeesTable { get; set; }
        /* */


        // Таблиці насіння
        /* */
        public DbSet<SeedModel> SeedTable { get; set; }

        public DbSet<UseCERTModel> UseCERTTable { get; set; }
        public DbSet<CERTModel> CERTTable { get; set; }
        public DbSet<CERTFileModel> CERTFileTable { get; set; }
        public DbSet<CERTAdditionalModel> CERTAdditionalTable { get; set; }

        public DbSet<SolanaSeedModel> SolanaSeedTable { get; set; }
        public DbSet<SolanaQrCodeModel> SolanaQrCodeTable { get; set; }
        public DbSet<SignatureModel> SignatureTablse { get; set; }
        /* */
    }
}