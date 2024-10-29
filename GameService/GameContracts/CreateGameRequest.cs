using GameService.Models;

namespace GameService.Contracts
{
    public record CreateGameRequest(
    string title,
    float price,
    string category,
    string description,
    IFormFile file, 
    string altText);
}
