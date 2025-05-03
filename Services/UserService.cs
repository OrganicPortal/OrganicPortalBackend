using CYberCryptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using OrganicPortalBackend.Controllers;
using OrganicPortalBackend.Models.Database;
using OrganicPortalBackend.Models.Database.User;
using OrganicPortalBackend.Models.Options;
using OrganicPortalBackend.Services.Response;

namespace OrganicPortalBackend.Services
{
    public interface IUser
    {
        // Користувацькі методи
        /* */
        public Task<ResponseFormatter> MyProfileAsync(string token);
        public Task<ResponseFormatter> EditMyProfileAsync(EditUserProfileIncomingObj incomingObj, string token);
        /* */


        // Адміністративні методи
        /* */
        public Task<ResponseFormatter> UserProfileAsync(long userId);
        /* */
    }
    public class UserService : IUser
    {
        public readonly OrganicContext _dbContext;
        public readonly TokenService _tokenService;
        public readonly EncryptOptions _encryptOptions;
        public UserService(OrganicContext dbContext, TokenService tokenService, IOptions<EncryptOptions> encryptOptions)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;

            _encryptOptions = encryptOptions.Value;
        }


        // Користувацькі методи
        /* */
        public async Task<ResponseFormatter> MyProfileAsync(string token)
        {
            long userId = _tokenService.GetUserIdFromLoginToken(token);

            return await UserProfileAsync(userId);
        }
        public async Task<ResponseFormatter> EditMyProfileAsync(EditUserProfileIncomingObj incomingObj, string token)
        {
            long userId = _tokenService.GetUserIdFromLoginToken(token);

            if (userId != incomingObj.UserId)
                return new ResponseFormatter(message: "Ідентифікатор користувача не правильний.");

            return await EditMyProfileAsync(incomingObj, token);
        }
        /* */


        // Адміністративні методи
        /* */
        public async Task<ResponseFormatter> UserProfileAsync(long userId)
        {
            CYberFormatter cyberFormatter = new CYberFormatter();
            string key = cyberFormatter.EncryptMethod(userId.ToString(), _encryptOptions.Key);

            var phone = await _dbContext.PhoneTable
                        .Where(item => EF.Functions.Collate(item.Key, "SQL_Latin1_General_CP1_CS_AS") == key)
                        .Where(item => item.IsActive)
                        .Select(item => item.Phone)
                        .FirstOrDefaultAsync() ?? "";

            if (!string.IsNullOrWhiteSpace(phone))
                phone = cyberFormatter.DecryptMethod(phone, _encryptOptions.PhoneKey);

            var user = await _dbContext.UserTable
                .Include(item => item.CompanyList)
                .ThenInclude(item => item.Company)

                .Select(item => new
                {
                    item.Id,
                    item.FirstName,
                    item.MiddleName,
                    item.LastName,
                    item.CreatedDate,

                    Phone = phone,

                    CompanyList = item.CompanyList
                    .Select(comp => new
                    {
                        comp.CreatedDate,
                        comp.CompanyId,
                        comp.Role,

                        CompanyName = comp.Company!.Name,
                        CompanyArchivated = comp.Company!.isArchivated,
                    })
                    .ToList()

                })
                .FirstOrDefaultAsync();

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK, data: user);
        }
        public async Task<ResponseFormatter> EditProfileAsync(EditUserProfileIncomingObj incomingObj, string token)
        {
            var user = await _dbContext.UserTable.FirstOrDefaultAsync(item => item.Id == incomingObj.UserId);

            if (user == null)
                return new ResponseFormatter(message: "Користувача не існує.");

            user.FirstName = incomingObj.FirstName;
            user.MiddleName = incomingObj.MiddleName;
            user.LastName = incomingObj.LastName;

            _dbContext.UserTable.Update(user);
            await _dbContext.SaveChangesAsync();

            return new ResponseFormatter(type: System.Net.HttpStatusCode.OK);
        }
        /* */
    }
}
