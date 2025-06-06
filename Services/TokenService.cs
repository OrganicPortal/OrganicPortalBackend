using CYberCryptor;
using OrganicPortalBackend.Models.Database.User.Session;
using OrganicPortalBackend.Models.Options;
using System.Text.Json;

namespace OrganicPortalBackend.Services
{
    // Статичний клас отримання користувацького ідентифікатора
    public static class TokenService
    {
        private static string _tokenKey;
        private static CYberFormatter _cyberFormatter = new CYberFormatter();

        // Процедура ініціалізації
        public static void Init(IConfiguration _configuration)
        {
            _tokenKey = _configuration.GetSection("EncryptOptions").Get<EncryptOptions>()!.TokenKey;
        }

        // Функція отримання користувацького ідентифікатора
        public static long GetUserIdFromLoginToken(string token)
        {
            var value = _cyberFormatter.DecryptMethod(token, _tokenKey);
            return JsonSerializer.Deserialize<TokenInformation>(value)!.UserId;
        }
    }
}
