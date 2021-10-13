using System;
using System.Threading.Tasks;

namespace UrlShortener.Business.Services
{
    public interface IShortUrlService
    {
        Task<string> CreateShortUrlKeyAsync(string url);

        Task<string> CreateShortUrlKeyAsync(Uri url);

        Task<Uri> GetUrlAsync(string urlKey);
    }
}