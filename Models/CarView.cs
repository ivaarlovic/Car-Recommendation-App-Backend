namespace CarRecommendationApp.Models
{
    public class CarView
    {
        public int Id { get; set; }
        public int SurveyUserId { get; set; }
        public int CarId { get; set; }
        public DateTime ViewedAt { get; set; } 

    }
}
