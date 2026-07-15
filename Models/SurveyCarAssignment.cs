namespace CarRecommendationApp.Models
{
    public class SurveyCarAssignment
    {
        public int Id { get; set; }

        public int SurveyUserId { get; set; }

        public int CarId { get; set; }

        public DateTime AssignedAt { get; set; }
    }
}