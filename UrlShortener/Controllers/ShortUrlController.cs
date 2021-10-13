using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UrlShortener.Business;
using UrlShortener.Business.Services;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    [Route("/")]
    public class ShortUrlController : Controller
    {
        private readonly IShortUrlService _shortUrlService;
        private readonly ILogger<ShortUrlController> _logger;

        public ShortUrlController(
            IShortUrlService shortUrlService,
            ILogger<ShortUrlController> logger)
        {
            _shortUrlService = shortUrlService ?? throw new ArgumentNullException(nameof(shortUrlService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateShortUrlModel model)
        {
            if (model.Uri is null)
            {
                ModelState.AddModelError(nameof(model.Uri), "The Uri must not be empty.");
                return View(model);
            }

            string? key = default;
            try
            {
                key = await _shortUrlService.CreateShortUrlKeyAsync(model.Uri);
            }
            catch (ShortUrlException ex)
            {
                _logger.LogWarning(ex, "Unable to generate URL");

                return Error();
            }

            var result = new ShortUrlModel
            {
                LongUri = model.Uri,
                //ShortUri = new Uri(Url.ActionLink(nameof(UrlRoutingController.Index), nameof(UrlRoutingController), new { urlKey = key }))
                ShortUri = new Uri(Url.Link("ShortUrlRedirect", new { urlKey = key })!)
            };

            return RedirectToAction(nameof(Get), result);
        }

        [HttpGet]
        [Route("url")]
        public IActionResult Get(ShortUrlModel model)
        {
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}