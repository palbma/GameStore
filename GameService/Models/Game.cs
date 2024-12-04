namespace GameService.Models
{
    public class Game
    {
        public Guid GameId { get; set; }
        public string Title { get; set; } = string.Empty;
        public float Price {  get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? ImageId { get; set; } 
        public GameImage? Image { get; set; }

        public Game(string title, float price, string category, string description)
        {
            Title = title;
            Price = price;
            Category = category;
            Description = description;
        }
    }
}
