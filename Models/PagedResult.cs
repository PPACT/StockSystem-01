namespace StockSystem.Models
{
    public class PagedResult<T>
    {
        public List<T> List { get; set; } = new List<T>();
        public int Total { get; set; }
    }
}