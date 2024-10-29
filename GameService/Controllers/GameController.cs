using Azure.Core;
using GameService.Contracts;
using GameService.GameContracts;
using GameService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Linq.Expressions;
using static Azure.Core.HttpHeader;
using static System.Net.Mime.MediaTypeNames;
namespace GameService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public GameController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpPost]
        public async Task<IActionResult> CreateGame([FromForm] CreateGameRequest request, CancellationToken ct)
        {
            var imagePath = await SaveImageAsync(request.file, ct);
            var thumpnailPath = await SaveThumbnailAsync(imagePath, ct);

            var gameInfo = new Game(
                request.title,
                request.price,
                request.category,
                request.description);

            await _dbContext.Games.AddAsync(gameInfo, ct);
            await _dbContext.SaveChangesAsync(ct);

            var gameImage = new GameImage(
             imagePath,
             thumpnailPath,
             request.altText)
            {
                GameId = gameInfo.GameId,
                Game = gameInfo
            };

            gameImage.Game = gameInfo;
            gameImage.GameId = gameInfo.GameId;

            await _dbContext.GameImages.AddAsync(gameImage, ct);
            await _dbContext.SaveChangesAsync(ct);


            return Ok();
        }
        private async Task<string> SaveThumbnailAsync(string imagePath, CancellationToken ct)
        {
            var thumbnailPath = Path.Combine("Uploads", "thumbnails", Path.GetFileName(imagePath));

            if (!Directory.Exists("Uploads/thumbnails"))
            {
                Directory.CreateDirectory("Uploads/thumbnails");
            }

            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(imagePath, ct))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(150, 150),
                    Mode = ResizeMode.Crop
                }));


                await image.SaveAsync(thumbnailPath, ct);
            }

            return thumbnailPath;
        }
        private async Task<string> SaveImageAsync(IFormFile imageFile, CancellationToken ct)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image file is required.");

            var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
            var uploadPath = Path.Combine("Uploads", fileName);

            if (!Directory.Exists("Uploads"))
            {
                Directory.CreateDirectory("Uploads");
            }

            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream, ct);
            }

            return uploadPath;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetGameRequest request, CancellationToken ct)
        {
            var gamesQuery = _dbContext.Games.Where(n => string.IsNullOrEmpty(request.Search)
            || n.Title.ToLower().Contains(request.Search.ToLower()));

            switch (request.SortOrder)
            {
                case "desc":
                    gamesQuery = gamesQuery.OrderByDescending(SortByItem(request.SortItem));
                    break;
                default:
                    gamesQuery = gamesQuery.OrderBy(SortByItem(request.SortItem));
                    break;
            }

            var games = await gamesQuery.ToListAsync(cancellationToken: ct);
            if (!games.Any())
            {
                return NotFound("No notes found.");
            }
            var gameDtos = await gamesQuery.Select(game => new GameDto(
                    game.GameId,
                    game.Title,
                    game.Price,
                    game.Category,
                    game.Description,
                    game.Image.ImagePath,
                    game.Image.ThumbnailPath,
                    game.Image.AltText
                )).ToListAsync(ct);

            return Ok(new GetGameResponse(gameDtos));
        }
        private Expression<Func<Game, object>> SortByItem(string? sortItem)
        {
            Expression<Func<Game, object>> selectorKey;
            switch (sortItem?.ToLower())
            {
                case "price":
                    selectorKey = game => game.Price;
                    break;
                case "title":
                    selectorKey = game => game.Title;
                    break;
                default:
                    selectorKey = game => game.GameId;
                    break;
            }
            return selectorKey;
        }
        [HttpDelete("{gameId}")]
        public async Task<IActionResult> DeleteGame(Guid gameId)
        {
            var game = await _dbContext.Games.FindAsync(gameId);


            if (game == null)
            {
                return NotFound(new { message = $"Game with ID {gameId} not found." });
            }

            if (game.ImageId.HasValue)
            {
                var image = await _dbContext.GameImages.FindAsync(game.ImageId.Value);

                if (image != null)
                {
                    _dbContext.GameImages.Remove(image);
                }
            }
            else
            {
                return NotFound(new { message = "Not found" });
            }

            _dbContext.Games.Remove(game);

            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        // fix it cause crivo napisano
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateGame(Guid id, [FromForm] UpdateGameRequest updateGameRequest, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var game = _dbContext.Games.Find(id);
            if (game == null)
            {
                return NotFound("game not foudn");
            }

            var image = _dbContext.GameImages.Find(game.ImageId.Value);

            if (image == null)
            {
                return NotFound("image not foudn");
            }

            UpdateGameFields(game, updateGameRequest);


            UpdateImageFile(image, updateGameRequest, ct);


            _dbContext.GameImages.Update(image);
            _dbContext.Games.Update(game);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
        private void UpdateGameFields(Game game, UpdateGameRequest updateGameRequest)
        {
            if (!string.IsNullOrWhiteSpace(updateGameRequest.title))
            {
                game.Title = updateGameRequest.title;
            }
            if (updateGameRequest.price.HasValue)
            {
                game.Price = updateGameRequest.price.Value;
            }
            if (!string.IsNullOrWhiteSpace(updateGameRequest.category))
            {
                game.Category = updateGameRequest.category;
            }
            if (!string.IsNullOrWhiteSpace(updateGameRequest.description))
            {
                game.Description = updateGameRequest.description;
            }
        }
        private async void UpdateImageFile(GameImage image, UpdateGameRequest updateGameRequest, CancellationToken ct) 
        {
            if (updateGameRequest.file != null)
            {
                var newImagePath = await SaveImageAsync(updateGameRequest.file, ct);
                var newThumbnailPath = await SaveThumbnailAsync(newImagePath, ct);

                if (System.IO.File.Exists(image.ImagePath))
                {
                    System.IO.File.Delete(image.ImagePath);
                }
                if (System.IO.File.Exists(image.ThumbnailPath))
                {
                    System.IO.File.Delete(image.ThumbnailPath);
                }

                image.ImagePath = newImagePath;
                image.ThumbnailPath = newThumbnailPath;
            }
        }
    }
}
