using Microsoft.EntityFrameworkCore;
using CarRecommendationApp.Models;


namespace CarRecommendationApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { 
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Rating> Ratings { get; set; } 
        public DbSet<SurveyUser> SurveyUsers { get; set; }
        public DbSet<SurveyRating> SurveyRatings { get; set; }
        public DbSet<CarView> CarViews { get; set; }
        public DbSet<UserCarPreferences> UserCarPreferences { get; set; }
    }
}
