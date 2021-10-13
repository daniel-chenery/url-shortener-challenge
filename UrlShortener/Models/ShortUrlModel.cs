using System;

namespace UrlShortener.Models
{
    public class ShortUrlModel
    {
        public Uri? LongUri { get; set; }

        public Uri? ShortUri { get; set; }
    }
}