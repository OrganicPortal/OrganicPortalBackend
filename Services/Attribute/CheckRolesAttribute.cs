using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganicPortalBackend.Models.Database;
using OrganicPortalBackend.Models.Database.Company;
using CYberCryptor;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OrganicPortalBackend.Models.Options;
using OrganicPortalBackend.Models.Database.User.Session;

namespace OrganicPortalBackend.Services.Attribute
{
    public class RolesAttribute : TypeFilterAttribute
    {
        public RolesAttribute(bool useCompanyId = false, params EnumUserRole[] roles) : base(typeof(RolesFilter))
        {
            Arguments = new object[] { useCompanyId, roles };
        }
    }
    public class RolesFilter : IAsyncAuthorizationFilter
    {
        public OrganicContext _dbContext;
        public bool UseCompanyId { get; set; } = false;
        public List<EnumUserRole> Roles { get; set; }
        public readonly string _tokenKey;

        public RolesFilter(OrganicContext dbContext, IOptions<EncryptOptions> encryptOptions, bool useCompanyId, EnumUserRole[] roles)
        {
            _dbContext = dbContext;
            UseCompanyId = useCompanyId;
            _tokenKey = encryptOptions.Value.TokenKey;
            Roles = roles.ToList();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] ?? "";

            if (!string.IsNullOrWhiteSpace(token))
            {
                try
                {
                    CYberFormatter cyberFormatter = new CYberFormatter();
                    long userId = JsonSerializer.Deserialize<TokenInformation>(cyberFormatter.DecryptMethod(token, _tokenKey))!.UserId;

                    if (userId > 0)
                    {
                        if (UseCompanyId == true)
                        {
                            long companyId = GetCompanyId(context.HttpContext);

                            if (companyId > 0)
                            {
                                bool isValid = await _dbContext.EmployeesTable
                                    .Where(item => item.UserId == userId)
                                    .Where(item => Roles.AsQueryable().Contains(item.Role))
                                    .AnyAsync();

                                if (isValid)
                                    return;
                            }
                        }
                    }
                }
                catch { }
            }

            context.Result = new BadRequestResult();
        }

        private long GetCompanyId(HttpContext httpContext)
        {
            long companyId = 0;

            if (long.TryParse(httpContext.Request.Query["companyId"], out companyId))
                return companyId;

            return companyId;
        }
    }
}
