using Moq;
using NUnit.Framework;
using System.Data;
using System.Threading.Tasks;
using UrlShortener.Data.Factories;
using UrlShortener.Data.Models;
using UrlShortener.Data.Repositories;

namespace UrlShortener.Data.Tests.Repositories
{
    [TestFixture]
    public class RepositoryTests
    {
        [Test]
        public void GetAsync_Throws_For_No_Record()
        {
            // arrange
            var repository = GetRepository<ShortUrl>();

            // act
            Task Act() => repository.GetAsync(0);

            // assert
            Assert.That(Act, Throws.InstanceOf<DataException>());
        }

        private IRepository<int, T> GetRepository<T>()
            where T : Entity<int>
            => new Repository<int, T>(Mock.Of<IConnectionFactory>());
    }
}