using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using OrganicPortalBackend.Models.Database;

namespace OrganicPortalBackend.Services.Attribute
{
    public class AuthorizedAttribute : TypeFilterAttribute
    {
        public AuthorizedAttribute() : base(typeof(AuthorizedFilter)) { }
    }
    public class AuthorizedFilter : IAsyncAuthorizationFilter
    {
        public OrganicContext _dbContext;

        public AuthorizedFilter(OrganicContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] ?? "";

            if (!string.IsNullOrWhiteSpace(token))
            {
                var dbToken = await _dbContext.SessionTable
                    .Where(item => EF.Functions.Collate(item.Token, "SQL_Latin1_General_CP1_CS_AS") == token)
                    .Where(item => item.ExpiredDate >= DateTime.UtcNow)
                    .FirstOrDefaultAsync();


                if (dbToken != null)
                {
                    dbToken.LastActivityDate = DateTime.UtcNow;
                    dbToken.ExpiredDate = DateTime.UtcNow;

                    _dbContext.SessionTable.Update(dbToken);
                    await _dbContext.SaveChangesAsync();

                    return;
                }
            }

            context.Result = new UnauthorizedResult();
        }
    }
}
