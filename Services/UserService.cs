using CYberCryptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrganicPortalBackend.Controllers;
using OrganicPortalBackend.Models.Database;
using OrganicPortalBackend.Models.Database.Company;
using OrganicPortalBackend.Models.Options;
using OrganicPortalBackend.Services.Response;

namespace OrganicPortalBackend.Services
{
    public interface IUser
    {
        // Користувацькі методи
        /* */
        // Функція отримання профілю авторизованого користувача
        public Task<ResponseFormatter> MyProfileAsync(string token);

        // Функція редагування профілю користувача
        public Task<ResponseFormatter> EditMyProfileAsync(EditUserProfileIncomingObj incomingObj, string token);
        /* */


        // Адміністративні методи
        /* */
        // Функція отримання профіля користувача за userId
        public Task<ResponseFormatter> UserProfileAsync(long userId);
        /* */
    }


    public class UserService : IUser
    {
        public readonly IDbContextFactory<OrganicContext> _dbContextFactory;
        public readonly OrganicContext _dbContext;
        public readonly EncryptOptions _encryptOptions;
        public UserService(OrganicContext dbContext, IOptions<EncryptOptions> encryptOptions, IDbContextFactory<OrganicContext> dbContextFactory)
        {
            _dbContext = dbContext;

            _encryptOptions = encryptOptions.Value;
            _dbContextFactory = dbContextFactory;
        }


        // Користувацькі методи
        /* */
        // Функція отримання профілю авторизованого користувача
        public async Task<ResponseFormatter> MyProfileAsync(string token)
        {
            // Отримуємо користувацький ідентифікатор
            long userId = TokenService.GetUserIdFromLoginToken(token);

            return await UserProfileAsync(userId);
        }

        // Функція редагування профілю користувача
        public async Task<ResponseFormatter> EditMyProfileAsync(EditUserProfileIncomingObj incomingObj, string token)
        {
            // Отримуємо користувацький ідентифікатор
            long userId = TokenService.GetUserIdFromLoginToken(token);

            if (userId != incomingObj.UserId)
                //return new ResponseFormatter(message: "Ідентифікатор користувача не правильний.");
                return new ResponseFormatter(message: "The user ID is incorrect.");

            return await EditProfileAsync(incomingObj, token);
        }
        /* */


        // Адміністративні методи
        /* */
        // Функція отримання профіля користувача за userId
        public async Task<ResponseFormatter> UserProfileAsync(long userId)
        {
            // Отримуємо ключ по ідентифікатору користувача
            CYberFormatter cyberFormatter = new CYberFormatter();
            string key = cyberFormatter.EncryptMethod(userId.ToString(), _encryptOptions.Key);

            // Запит на отримання номеру телефону корситувача
            Task<string> phoneTask = Task<string>.Run(() =>
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    return dbContext.PhoneTable
                            .Where(item => EF.Functions.Collate(item.Key, "SQL_Latin1_General_CP1_CS_AS") == key)
                            .Where(item => item.IsActive)
                            .Select(item => item.Phone)
                            .FirstOrDefault() ?? "";
                }
            });

            // Запит на отримання користувача
            Task<UserProfileOutcomingObj?> userTask = Task<UserProfileOutcomingObj?>.Run(() =>
            {
                using (var dbContext = _dbContextFactory.CreateDbContext())
                {
                    return dbContext.UserTable
                        .Include(item => item.CompanyList)
                        .ThenInclude(item => item.Company)

                        .Where(item => item.Id == userId)
                        .Select(item => new UserProfileOutcomingObj
                        {
                            Id = item.Id,
                            FirstName = item.FirstName,
                            MiddleName = item.MiddleName,
                            LastName = item.LastName,
                            CreatedDate = item.CreatedDate,

                            CompanyList = item.CompanyList
                            .Select(comp => new CompanyProfileOutcomingObj
                            {
                                CompanyId = comp.CompanyId,
                                CreatedDate = comp.CreatedDate,
                                Role = comp.Role,

                                CompanyName = comp.Company!.Name,
                                CompanyArchivated = comp.Company!.isArchivated,
                            })
                            .ToList()

                        })
                        .FirstOrDefault();
                }
            });

            // Очикуємо виконання
            await Task.WhenAll(phoneTask, userTask);

            // Отримуємо результати
            string phone = phoneTask.Result;
            UserProfileOutcomingObj? user = userTask.Result;

            // Оновлюємо номер телефону
            if (!string.IsNullOrWhiteSpace(phone) && user != null)
                user.Phone = cyberFormatter.DecryptMethod(phone, _encryptOptions.PhoneKey);

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK, data: user);
        }

        // Функція редагування профілю користувача за userId
        public async Task<ResponseFormatter> EditProfileAsync(EditUserProfileIncomingObj incomingObj, string token)
        {
            // Отримуємо запис профілю
            var user = await _dbContext.UserTable.FirstOrDefaultAsync(item => item.Id == incomingObj.UserId);

            if (user == null)
                //return new ResponseFormatter(message: "Користувача не існує.");
                return new ResponseFormatter(message: "The user does not exist.");

            // Редагуємо профіль
            user.FirstName = incomingObj.FirstName;
            user.MiddleName = incomingObj.MiddleName;
            user.LastName = incomingObj.LastName;

            _dbContext.UserTable.Update(user);
            await _dbContext.SaveChangesAsync();

            //return new ResponseFormatter(type: System.Net.HttpStatusCode.OK, "Дані успішно відредаговано");
            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK, "Data successfully edited");
        }
        /* */
    }


    // Користувацькі вихідіні об'єкти
    /* */
    // Вихідний об'єкт, з інформацією по профілю користувача
    public class UserProfileOutcomingObj
    {
        // Ідентифікатор користувача
        // #exemple::1025
        public long Id { get; set; } = 0;

        // Ім'я користувача
        // #exemple::Сергій
        public string FirstName { get; set; } = string.Empty;

        // По батькові користувача
        // #exemple::Вікторович
        public string MiddleName { get; set; } = string.Empty;

        // Прізвище користувача
        // #exemple::Глушков
        public string LastName { get; set; } = string.Empty;

        // Дата реєстрації користувача
        // #exemple::2001 рік
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Номер телефону користувача
        // #exemple::+380689876545
        public string Phone { get; set; } = string.Empty;

        // Список компаній користувача
        public ICollection<CompanyProfileOutcomingObj> CompanyList { get; set; } = new List<CompanyProfileOutcomingObj>();
    }

    // Вихідний об'єкт, з інформацією по компанії користувача
    public class CompanyProfileOutcomingObj
    {
        // Ідентифікатор компанії
        // #exemple::156
        public long CompanyId { get; set; } = 0;

        // Ім'я компанії
        // #exemple::ТМ Яскрава
        public string CompanyName { get; set; } = string.Empty;

        // Чи заархівована компанія
        // #exemple::false
        public bool CompanyArchivated { get; set; } = false;

        // Дата створення компанії
        // #exemple::2001 рік
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Роль користувача в компанії
        // #exemple::10 (Owner)
        public EnumUserRole Role { get; set; } = EnumUserRole.User;
    }
    /* */
}
