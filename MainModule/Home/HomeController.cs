// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Duende.IdentityServer.Services;
using StepIn.Services.Identity;

namespace IdentityServerHost.Quickstart.UI
{
  [SecurityHeaders]
  [AllowAnonymous]
  public class HomeController : Controller
  {
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger _logger;

    private readonly IConfiguration _configuration;

    public HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment environment, ILogger<HomeController> logger, IConfiguration configuration)
    {
      _interaction = interaction;
      _environment = environment;
      _logger = logger;
      _configuration = configuration;
    }

    public IActionResult Index()
    {
      if (_environment.IsDevelopment())
      {
        // only show in development

        TempData["FrontOfficeHomePage"] = _configuration.GetValue<string>("FrontOfficeHomePage");
        // TempData["RedirectUris"] = SD.RedirectUris();
        // TempData["PostLogoutRedirectUris"] = SD.PostLogoutRedirectUris();

        // _logger.LogInformation("RedirectUris:<{0}>", SD.RedirectUris());

        _logger.LogInformation("FrontOfficeHomePage : {0}", TempData["FrontOfficeHomePage"]);

        return View();
      }

      _logger.LogInformation("Homepage is disabled in production. Returning 404.");
      return NotFound();
    }

    /// <summary>
    /// Shows the error page
    /// </summary>
    public async Task<IActionResult> Error(string errorId)
    {
      var vm = new ErrorViewModel();

      // retrieve error details from identityserver
      var message = await _interaction.GetErrorContextAsync(errorId);
      if (message != null)
      {
        vm.Error = message;

        if (!_environment.IsDevelopment())
        {
          // only show in development
          message.ErrorDescription = null;
        }
      }

      return View("Error", vm);
    }
  }
}