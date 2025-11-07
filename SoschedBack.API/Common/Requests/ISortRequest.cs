namespace SoschedBack.Common.Requests;

public interface ISortRequest
{
    string? SortBy { get; }
    
    bool Descending { get; }
}