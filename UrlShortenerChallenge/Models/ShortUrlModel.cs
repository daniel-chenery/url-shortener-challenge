using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UrlShortener.Models
{
    public class ShortUrlModel
    {
        public Uri? LongUri { get; set; }

        public Uri? ShortUri { get; set; }
    }
}