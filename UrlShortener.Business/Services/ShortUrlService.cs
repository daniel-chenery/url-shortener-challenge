using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortener.Core.Configuration;
using UrlShortener.Core.Services;
using UrlShortener.Data;
using UrlShortener.Data.Models;
using UrlShortener.Data.Repositories;

namespace UrlShortener.Business.Services
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly IRepository<int, ShortUrl> _repository;
        private readonly IUrlCharacterService _urlCharacterService;
        private readonly IOptions<UrlGenerationOptions> _options;
        private readonly ILogger<ShortUrlService> _logger;

        private readonly Random _random = new();

        public ShortUrlService(
            IRepository<int, ShortUrl> repository,
            IUrlCharacterService urlCharacterService,
            IOptions<UrlGenerationOptions> options,
            ILogger<ShortUrlService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _urlCharacterService = urlCharacterService ?? throw new ArgumentNullException(nameof(urlCharacterService));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<string> CreateShortUrlKeyAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("The url provided must not be null or empty.", nameof(url));
            }

            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                throw new ArgumentException("The url provided is not formatted correctly.", nameof(url));
            }

            return CreateShortUrlKeyAsync(uri);
        }

        public virtual async Task<string> CreateShortUrlKeyAsync(Uri url)
        {
            try
            {
                if (await _repository.ExistsAsync(su => su.Url == url))
                {
                    return (await _repository.GetAsync(su => su.Url == url)).UrlKey ?? throw new ShortUrlException("Unable to find short url key.");
                }
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Unable to retrieve records from database.");

                throw new ShortUrlException("Unable to find short url key.", ex);
            }

            string? key = default;

            var attempts = 0;
            try
            {
                do
                {
                    ++attempts;
                    key = await GenerateUrlKey();
                } while (await _repository.ExistsAsync(su => su.UrlKey == key) && attempts < _options.Value.MaximumGenerateAttempts);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Failed to generate URL Key after {attempts} attempts.");

                throw new ShortUrlException("Unable to generate short url key.", ex);
            }

            try
            {
                await _repository.InsertAsync(new ShortUrl
                {
                    Url = url,
                    UrlKey = key
                });
            }
            catch (DataException ex)
            {
                _logger.LogError(ex, "Failed to save short url to database.");
                throw new ShortUrlException("Unable to save short url key.", ex);
            }

            return key;
        }

        private async Task<string> GenerateUrlKey()
        {
            // Get the last dB record to check the key length
            // This will prevent us needlessly looping over endless records
            var lastRecord = await _repository.GetAllAsync(_ => true, new QueryOptions
            {
                Limit = 1,
                Order = Order.Descending
            });
            var minLength = Math.Max(_options.Value.MinimumUrlLength, lastRecord?.SingleOrDefault()?.UrlKey?.Length ?? 0);

            if (minLength <= 0)
            {
                throw new InvalidOperationException("The minimum length must be greater than 0.");
            }

            var validChars = _urlCharacterService
                .GetValidCharacters()
                .ToList();
            var selectedChars = new List<char>();

            for (var i = 0; i < minLength; ++i)
            {
                selectedChars.Add(validChars.ElementAt(_random.Next(validChars.Count)));
            }

            if (selectedChars.Count < minLength)
            {
                throw new InvalidOperationException("No characters available to generate url key.");
            }

            return new string(selectedChars.ToArray());
        }

        public async Task<Uri> GetUrlAsync(string urlKey)
        {
            var shortUrl = await _repository.GetAsync(su => su.UrlKey == urlKey);

            if (shortUrl is null)
            {
                throw new ShortUrlException($"The url for {urlKey} could not be found.");
            }

            return shortUrl.Url!;
        }
    }
}