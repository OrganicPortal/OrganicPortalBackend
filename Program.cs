using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
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

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
});

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
builder.Services.AddScoped<ICompany, CompanyService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddScoped<ISeed, SeedService>();
builder.Services.AddScoped<TokenService>();

builder.Services.Configure<EncryptOptions>(builder.Configuration.GetSection("EncryptOptions"));
builder.Services.Configure<SMSOptions>(builder.Configuration.GetSection("SMSServiceOptions"));

builder.Services.AddAuthorization();



var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("MyAllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
