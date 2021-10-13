using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UrlShortener.Business.Services;
using UrlShortener.Core.Configuration;
using UrlShortener.Core.Services;
using UrlShortener.Data;
using UrlShortener.Data.Models;
using UrlShortener.Data.Repositories;

namespace UrlShortener.Business.Tests.Services
{
    [TestFixture]
    public class ShortUrlServiceTests
    {
        private Mock<IRepository<int, ShortUrl>> _mockRepository;
        private Mock<IUrlCharacterService> _mockUrlCharacterService;
        private UrlGenerationOptions _options;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new();
            _mockUrlCharacterService = new();
            _options = new();
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void CreateUrl_String_Throws_For_Empty_Or_Null(string url)
        {
            // arrange
            var service = GetService();

            // act
            Task Act() => service.CreateShortUrlKeyAsync(url);

            // assert
            Assert.That(Act, Throws.InstanceOf<ArgumentException>()
                .With.Property(nameof(ArgumentException.ParamName)).EqualTo("url"));
        }

        [TestCase("helloworld")]
        [TestCase("/endpoint")]
        [TestCase("/endpoint/")]
        public void CreateUrl_String_Throws_For_InvalidFormat(string url)
        {
            // arrange
            var service = GetService();

            // act
            Task Act() => service.CreateShortUrlKeyAsync(url);

            // assert
            Assert.That(Act, Throws.InstanceOf<ArgumentException>()
                .With.Property(nameof(ArgumentException.ParamName)).EqualTo("url")
                .And.Property(nameof(ArgumentException.Message)).Contains("format"));
        }

        [Test]
        public async Task CreateUrl_String_Calls_CreateUrl_Uri()
        {
            // arrange
            const string url = "https://www.example.com/";
            var mockService = new Mock<ShortUrlService>(
                _mockRepository.Object,
                _mockUrlCharacterService.Object,
                Mock.Of<IOptions<UrlGenerationOptions>>(o => o.Value == _options),
                Mock.Of<ILogger<ShortUrlService>>())
            {
                CallBase = true
            };

            // act
            try
            {
                // The result of this isn't important, we just want to make sure it called the Uri overload
                _ = await mockService.Object.CreateShortUrlKeyAsync(url);
            }
            catch { }

            // assert
            mockService.Verify(sus => sus.CreateShortUrlKeyAsync(It.IsAny<Uri>()), Times.Once);
        }

        [TestCase("\\\\external-server")]
        [TestCase("https://www.example.com:443")]
        [TestCase("https://www.example.com")]
        [TestCase("http://www.example.com")]
        public async Task CreateUrl_With_Uri_Calls_Repository_And_Returns_ShortKey(Uri uri)
        {
            // arrange
            var service = GetService();
            var shortUrl = default(ShortUrl);

            _mockRepository
                .Setup(r => r.InsertAsync(It.IsAny<ShortUrl>()))
                .Callback<ShortUrl>(su => shortUrl = su)
                .Returns(Task.CompletedTask);

            _mockRepository
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<ShortUrl, bool>>>()))
                .ReturnsAsync(false);

            _mockUrlCharacterService
                .Setup(r => r.GetValidCharacters())
                .Returns(new[] { 'a', 'b', 'c' });

            _mockRepository
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<ShortUrl, bool>>>(), It.IsAny<QueryOptions>()))
                .ReturnsAsync(Enumerable.Empty<ShortUrl>());

            // act
            var result = await service.CreateShortUrlKeyAsync(uri);

            // assert
            Assert.That(result, Is.Not.Null.Or.Empty);
            Assert.That(shortUrl, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(shortUrl.Url, Is.EqualTo(uri));
                Assert.That(shortUrl.UrlKey, Is.EqualTo(result));
            });
        }

        [Test]
        public async Task CreateUrl_MinLimitExceeded_Increases_Key_Length()
        {
            // arrange
            var service = GetService();
            var uri = new Uri("https://www.example.com");

            _options.MinimumUrlLength = 5;

            _mockUrlCharacterService
                .Setup(cs => cs.GetValidCharacters())
                .Returns(new[] { 'a' });

            _mockRepository
                .Setup(r => r.GetAllAsync(It.IsAny<Expression<Func<ShortUrl, bool>>>(), It.IsAny<QueryOptions>()))
                .ReturnsAsync(new[] { new ShortUrl { UrlKey = "123456" } });

            // act
            var result = await service.CreateShortUrlKeyAsync(uri);

            // assert
            Assert.That(result.Length, Is.EqualTo(6));
        }

        // There should be other test methods, checking the database results are returned and not new values
        // Alongside exceptions and "sad paths"

        private IShortUrlService GetService() =>
            new ShortUrlService(
                _mockRepository.Object,
                _mockUrlCharacterService.Object,
                Mock.Of<IOptions<UrlGenerationOptions>>(o => o.Value == _options),
                Mock.Of<ILogger<ShortUrlService>>());
    }
}