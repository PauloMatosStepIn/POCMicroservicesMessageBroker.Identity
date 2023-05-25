using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace StepIn.Services.Identity
{
  public static class SD
  {
    public const string Admin = "Admin";
    public const string Guest = "Guest";

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
                new ApiScope("stepin","StepIn Server" ),
                new ApiScope("read","Read your data" ),
                new ApiScope("write","Write your data" ),
                new ApiScope("delete","Delete your data" )
        };

  }
}
