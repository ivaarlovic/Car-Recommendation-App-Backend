namespace CarRecommendationApp.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string FuelType { get; set; } // petrol, diesel, electric
        public int HorsePower { get; set; }
        public string? ImageUrl { get; set; }
    }
}
