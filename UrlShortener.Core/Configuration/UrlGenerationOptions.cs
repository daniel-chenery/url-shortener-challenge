namespace UrlShortener.Core.Configuration
{
    public class UrlGenerationOptions
    {
        public const string SectionName = "UrlGeneration";

        public int MinimumUrlLength { get; set; } = 3;

        public int MaximumGenerateAttempts { get; set; } = 5;
    }
}