namespace CarRecommendationApp.Models
{
    public class UserCarPreferences
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int CarId { get; set; }

        public int Score { get; set; } = 10;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
