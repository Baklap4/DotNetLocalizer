using System;
using DotNetLocalizer.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DotNetLocalizer.Json.Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> localizer;

        public HomeController(IStringLocalizer<HomeController> localizer)
        {
            this.localizer = localizer;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            this.Response.Cookies.Append(
                                         CookieRequestCultureProvider.DefaultCookieName,
                                         CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                                         new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                                        );

            return this.LocalRedirect(returnUrl);
        }

        [HttpGet]
        public IActionResult DownloadMissingStrings()
        {
            var filestream = this.localizer.GetMissingLocalizedStrings();
            if (filestream == null)
            {
                return this.NotFound("MissingStringsFileNotFound");
            }
            return this.File(filestream, "application/json");
        }
    }
}
