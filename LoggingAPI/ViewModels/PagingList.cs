namespace LoggingAPI.ViewModels;

public class PagingList<T>
{
    public int TotalElements { get; set; }
    public int TotalPages { get; set; }
    public List<T> Items { get; set; }
}