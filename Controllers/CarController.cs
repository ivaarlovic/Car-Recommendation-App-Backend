using CarRecommendationApp.Data;
using Microsoft.AspNetCore.Mvc;
using CarRecommendationApp.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


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

        //get
        [HttpGet]
        public IActionResult GetCars()
        {
            var cars = _context.Cars.ToList();
            return Ok(cars);
        }

        //post
        [HttpPost]

        public IActionResult AddCar([FromBody] Car car)
        {
            _context.Cars.Add(car);
            _context.SaveChanges();

            return Ok(car);
        }

        public class PreferencesDto
        {
            public int UserId { get; set; }
            public List<int> CarIds { get; set; }
        }

        [HttpPost("save-preferences")]
        public async Task<IActionResult> SavePreferences([FromBody] PreferencesDto dto)
        {

            foreach (var carId in dto.CarIds)
            {
                var pref = new UserCarPreferences
                {
                    UserId = dto.UserId,
                    CarId = carId,
                    Score = 10
                };
                _context.UserCarPreferences.Add(pref);
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Odabir uspješno spremljen." });
        }
    }
}
