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
        public string Color { get; set; }
        public string Description { get; set; }
        public string Transmission { get; set; }
        public int Mileage { get; set; }
        public string BodyType { get; set; }
        public string Engine { get; set; }
    }
}
