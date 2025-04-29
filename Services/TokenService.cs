using CYberCryptor;
using Microsoft.Extensions.Options;
using OrganicPortalBackend.Models.Database.User.Session;
using OrganicPortalBackend.Models.Options;
using System.Text.Json;

namespace OrganicPortalBackend.Services
{
    public class TokenService
    {
        public readonly EncryptOptions _encryptOptions;
        public TokenService(IOptions<EncryptOptions> encryptOptions)
        {
            _encryptOptions = encryptOptions.Value;
        }

        public long GetUserIdFromLoginToken(string token)
        {
            CYberFormatter cyberFormatter = new CYberFormatter();
            string value = cyberFormatter.DecryptMethod(token, _encryptOptions.TokenKey);

            return JsonSerializer.Deserialize<TokenInformation>(value)!.UserId;
        }
    }
}
