using System;

namespace UrlShortener.Data.Models
{
    public class ShortUrl : Entity<int>
    {
        public Uri? Url { get; set; }

        public string? UrlKey { get; set; }
    }
}