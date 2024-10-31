namespace GameService.UserContracts
{
    public record GetGameRequest(string? Search,string? SortItem,string? SortOrder);
}
