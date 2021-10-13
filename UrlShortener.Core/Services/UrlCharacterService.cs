namespace UrlShortener.Core.Services
{
    public class UrlCharacterService : IUrlCharacterService
    {
        private const string Letters = "abcdefghijklmnopqrstuvwxyz";

        private const string Numbers = "1234567890";

        private const string Special = "_-";

        public char[] GetValidCharacters() => (Letters + Letters.ToUpperInvariant() + Numbers + Special).ToCharArray();
    }
}