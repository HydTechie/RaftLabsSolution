namespace RaftLabs.Users.Infrastructure.Models;

public class PaginatedResponse<T>
{
    public int Page { get; set; }
    public int Total_Pages { get; set; }
    public List<T> Data { get; set; } = new();
}