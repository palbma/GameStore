using GameService.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameService.GameContracts
{
    public record GameDto(
          Guid GameId,
          string Title,
          float Price,
          string Category,
          string Description,
          string ImagePath,      
          string ThumbnailPath,   
          string AltText);
}
