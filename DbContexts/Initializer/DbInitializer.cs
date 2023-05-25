using IdentityModel;
using StepIn.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using StepIn.Services.Identity.DbContexts;
using StepIn.Services.Identity.DbContexts.Initializer;
using StepIn.Services.Identity;

namespace StepIn.Services.Identity.DbContexts.Initializer
{
  public class DbInitializer : IDbInitializer
  {
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly IConfiguration _configuration;

    public DbInitializer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
      _db = db;
      _userManager = userManager;
      _roleManager = roleManager;
      _configuration = configuration;
    }

    public void Initialize()
    {

      if (_roleManager.FindByNameAsync(SD.Admin).Result == null)
      {
        _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
        _roleManager.CreateAsync(new IdentityRole(SD.Guest)).GetAwaiter().GetResult();
      }
      else { return; }

      ApplicationUser AdminUser = new ApplicationUser()
      {
        UserName = "admin",
        Email = "admin@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "987654321",
        FirstName = "Admin",
        LastName = "StepIn"
      };

      _userManager.CreateAsync(AdminUser, _configuration.GetValue<string>("AdminUserPassword")).GetAwaiter().GetResult();
      _userManager.AddToRoleAsync(AdminUser, SD.Admin).GetAwaiter().GetResult();

      var temp1 = _userManager.AddClaimsAsync(AdminUser, new Claim[] {
                new Claim(JwtClaimTypes.Name,AdminUser.FirstName+" "+AdminUser.LastName),
                new Claim(JwtClaimTypes.GivenName,AdminUser.FirstName),
                new Claim(JwtClaimTypes.FamilyName,AdminUser.LastName),
                new Claim(JwtClaimTypes.Role,SD.Admin)
            }).Result;

      ApplicationUser GuestUser = new ApplicationUser()
      {
        UserName = "guest",
        Email = "guest@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "961111111",
        FirstName = "Guest",
        LastName = "StepIn"
      };

      _userManager.CreateAsync(GuestUser, _configuration.GetValue<string>("GuestUserPassword")).GetAwaiter().GetResult();
      _userManager.AddToRoleAsync(GuestUser, SD.Guest).GetAwaiter().GetResult();

      var temp2 = _userManager.AddClaimsAsync(GuestUser, new Claim[] {
                  new Claim(JwtClaimTypes.Name,GuestUser.FirstName+" "+GuestUser.LastName),
                  new Claim(JwtClaimTypes.GivenName,GuestUser.FirstName),
                  new Claim(JwtClaimTypes.FamilyName,GuestUser.LastName),
                  new Claim(JwtClaimTypes.Role,SD.Admin)
              }).Result;
    }
  }
}
