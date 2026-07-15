using CarRecommendationApp.Data;
using CarRecommendationApp.Models;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CarRecommendationApp.Controllers
{
    [Route("api/import")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ImportController(AppDbContext context)
        {
            _context = context;
        }


        [HttpPost]
        public IActionResult ImportCars()
        {
            var filePath = "cars_ready.csv";


            using var reader = new StreamReader(filePath);

            using var csv = new CsvReader(
                reader,
                CultureInfo.InvariantCulture
            );


            var cars = csv.GetRecords<Car>().ToList();


            int added = 0;


            foreach (var car in cars)
            {
                bool exists = _context.Cars.Any(c =>
                    c.Brand == car.Brand &&
                    c.Model == car.Model &&
                    c.Year == car.Year
                );


                if (!exists)
                {
                    _context.Cars.Add(car);
                    added++;
                }
            }


            _context.SaveChanges();


            return Ok(new
            {
                message = "Import završen",
                dodano = added
            });
        }
    }
}