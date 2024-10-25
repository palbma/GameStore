namespace GameService.Contracts
{
    public record GetGameRequest(
        string? Search, 
        string? SortItem, 
        string? SortOrder);
}
