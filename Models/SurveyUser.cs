namespace CarRecommendationApp.Models
{
    public class SurveyUser
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsCompleted { get; set; }
        public bool HasConsented { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}
