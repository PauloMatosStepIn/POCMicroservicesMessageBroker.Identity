using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Services;
using StepIn.Services.Identity;
using StepIn.Services.Identity.DbContexts;
using StepIn.Services.Identity.DbContexts.Initializer;
using StepIn.Services.Identity.Models;
using StepIn.Services.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Duende.IdentityServer.Models;
using Duende.IdentityServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

string _redirectUris = builder.Configuration.GetValue<string>("RedirectUris");
string _postLogoutRedirectUris = builder.Configuration.GetValue<string>("PostLogoutRedirectUris");
string _clientSecret = builder.Configuration.GetValue<string>("ClientSecret");

IEnumerable<Client> Clients =
    new List<Client> {
                //Generic Client
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret(_clientSecret.Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = {"read", "write", "profle"}
                },
                new Client
                {
                    ClientId = "stepin",
                    ClientSecrets = { new Secret(_clientSecret.Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { _redirectUris },
                    PostLogoutRedirectUris = { _postLogoutRedirectUris },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "stepin"
                    }
                }
    };


var identityBuilder = builder.Services.AddIdentityServer(options =>
{
  options.Events.RaiseErrorEvents = true;
  options.Events.RaiseInformationEvents = true;
  options.Events.RaiseFailureEvents = true;
  options.Events.RaiseSuccessEvents = true;
  options.EmitStaticAudienceClaim = true;
}).AddInMemoryIdentityResources(SD.IdentityResources)
.AddInMemoryApiScopes(SD.ApiScopes)
.AddInMemoryClients(Clients)
.AddAspNetIdentity<ApplicationUser>();

identityBuilder.AddDeveloperSigningCredential();

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddScoped<IProfileService, ProfileService>();



var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
  var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
  dbInitializer.Initialize();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
