namespace GameService.GameContracts
{
    public record UpdateGameRequest(
    string? title,
    float? price,
    string? category,
    string? description,
    IFormFile? file
    );

}
