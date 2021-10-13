namespace UrlShortener.Data
{
    public class QueryOptions
    {
        public virtual Order Order { get; set; }

        public virtual int? Limit { get; set; }
    }
}