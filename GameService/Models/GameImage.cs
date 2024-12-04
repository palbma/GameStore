using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameService.Models
{
    public class GameImage
    {
        [Key]
        public Guid ImageId { get; set; }
        public string ImagePath {  get; set; } = string.Empty;
        public string ThumbnailPath { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
        
        public Game? Game { get; set; }
        public Guid? GameId { get; set; }

        public GameImage(string imagePath, string thumbnailPath, string altText)
        {
            ImagePath = imagePath;
            ThumbnailPath = thumbnailPath;
            AltText = altText;
        }
    }
}
