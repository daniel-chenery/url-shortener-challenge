namespace UrlShortener.Data.Models
{
    public abstract class Entity<TId>
    {
        public virtual TId? Id { get; set; }
    }
}