using CarRecommendationApp.Data;
using CarRecommendationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRecommendationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCars()
        {
            var cars = await _context.Cars.AsNoTracking().ToListAsync();
            return Ok(cars);
        }

        [HttpPost]
        public async Task<IActionResult> AddCar([FromBody] Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return Ok(car);
        }

        public class PreferencesDto
        {
            public int UserId { get; set; }
            public List<int> CarIds { get; set; } = new();
        }

        [HttpPost("save-preferences")]
        public async Task<IActionResult> SavePreferences([FromBody] PreferencesDto dto)
        {
            if (dto.UserId <= 0)
            {
                return BadRequest("Neispravan korisnik.");
            }

            var distinctCarIds = dto.CarIds.Distinct().ToList();

            if (distinctCarIds.Count != 5)
            {
                return BadRequest("Potrebno je odabrati točno 5 različitih automobila.");
            }

            var userExists = await _context.Users
                .AnyAsync(user => user.Id == dto.UserId);

            if (!userExists)
            {
                return NotFound("Korisnik ne postoji.");
            }

            var existingCarIds = await _context.Cars
                .Where(car => distinctCarIds.Contains(car.Id))
                .Select(car => car.Id)
                .ToListAsync();

            if (existingCarIds.Count != 5)
            {
                var missing = distinctCarIds.Except(existingCarIds).ToList();

                return BadRequest(new
                {
                    message = "Neki odabrani automobili ne postoje.",
                    missingCarIds = missing
                });
            }

            var oldPreferences = await _context.UserCarPreferences
                .Where(preference => preference.UserId == dto.UserId)
                .ToListAsync();

            _context.UserCarPreferences.RemoveRange(oldPreferences);

            var newPreferences = distinctCarIds.Select(carId =>
                new UserCarPreferences
                {
                    UserId = dto.UserId,
                    CarId = carId,
                    Score = 10,
                    CreatedAt = DateTime.UtcNow
                });

            await _context.UserCarPreferences.AddRangeAsync(newPreferences);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Odabir je uspješno spremljen.",
                carIds = distinctCarIds
            });
        }
    }
}
