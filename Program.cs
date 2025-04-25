using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using OrganicPortalBackend.Models.Database;
using OrganicPortalBackend.Models.Options;
using OrganicPortalBackend.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyAllowSpecificOrigins",
                      builder =>
                      {
                          builder
                              .WithOrigins("*")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              ;
                      });
});

builder.Services.AddDbContextFactory<OrganicContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")),
    ServiceLifetime.Transient
);

builder.Services.AddScoped<IAuthorization, AuthorizationService>();

builder.Services.Configure<EncryptOptions>(builder.Configuration.GetSection("EncryptOptions"));
builder.Services.Configure<SMSOptions>(builder.Configuration.GetSection("SMSServiceOptions"));


var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("MyAllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
