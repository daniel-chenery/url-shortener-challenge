using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using UrlShortener.Business.Services;

namespace UrlShortener.Controllers
{
    public class UrlRoutingController : Controller
    {
        private readonly IShortUrlService _shortUrlService;
        private readonly ILogger<UrlRoutingController> _logger;

        public UrlRoutingController(IShortUrlService shortUrlService, ILogger<UrlRoutingController> logger)
        {
            _shortUrlService = shortUrlService ?? throw new ArgumentNullException(nameof(shortUrlService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [Route("/{urlKey}", Name = "ShortUrlRedirect")]
        public async Task<IActionResult> Index(string urlKey)
        {
            Uri? uri;

            try
            {
                uri = await _shortUrlService.GetUrlAsync(urlKey);
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(ShortUrlController.Create), "ShortUrl");
            }

            if (uri is null)
            {
                _logger.LogWarning($"{urlKey} was not found.");
                return RedirectToAction(nameof(ShortUrlController.Create), "ShortUrl");
            }

            _logger.LogTrace($"Redirection /{urlKey} to {uri}");

            return RedirectPermanent(uri.ToString());
        }
    }
}