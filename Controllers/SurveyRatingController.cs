using CarRecommendationApp.Data;
using CarRecommendationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRecommendationApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyRatingController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public SurveyRatingController(
            AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/SurveyRating
        [HttpGet]
        public async Task<IActionResult> GetSurveyRatings()
        {
            var ratings = await _context.SurveyRatings
                .AsNoTracking()
                .ToListAsync();

            return Ok(ratings);
        }

        // GET: api/SurveyRating/user/1
        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetUserRatings(int userId)
        {
            var ratings = await _context.SurveyRatings
                .AsNoTracking()
                .Where(x => x.SurveyUserId == userId)
                .ToListAsync();

            return Ok(ratings);
        }

        // GET: api/SurveyRating/cars/1
        // Vraća 3 automobila iz postojećeg ocijenjenog skupa
        // i 27 automobila s najmanje ocjena.
        [HttpGet("cars/{surveyUserId:int}")]
        public async Task<IActionResult> GetSurveyCars(int surveyUserId)
        {
            const int totalCarsPerUser = 30;
            const int referenceCarsPerUser = 3;
            const int dynamicCarsPerUser =
                totalCarsPerUser - referenceCarsPerUser;

            if (surveyUserId <= 0)
            {
                return BadRequest("Neispravan korisnik ankete.");
            }

            var existingAssignments = await _context.SurveyCarAssignments
                .Where(x => x.SurveyUserId == surveyUserId)
                .OrderBy(x => x.Id)
                .ToListAsync();

            // Nakon osvježavanja korisnik dobiva isti skup od 30 automobila.
            if (existingAssignments.Count == totalCarsPerUser)
            {
                var existingIds = existingAssignments
                    .Select(x => x.CarId)
                    .ToList();

                var existingCars = await _context.Cars
                    .AsNoTracking()
                    .Where(x => existingIds.Contains(x.Id))
                    .ToListAsync();

                var order = existingIds
                    .Select((id, index) => new { id, index })
                    .ToDictionary(x => x.id, x => x.index);

                var orderedCars = existingCars
                    .OrderBy(x => order[x.Id])
                    .ToList();

                return Ok(orderedCars);
            }

            // Brišemo nepotpunu staru dodjelu.
            if (existingAssignments.Count > 0)
            {
                _context.SurveyCarAssignments
                    .RemoveRange(existingAssignments);

                await _context.SaveChangesAsync();
            }

            // Ovo su ID-jevi 22 automobila koji već imaju ocjene.
            var referenceCarIds = _configuration
                .GetSection("SurveySettings:ReferenceCarIds")
                .Get<List<int>>()?
                .Distinct()
                .ToList() ?? new List<int>();

            if (referenceCarIds.Count < referenceCarsPerUser)
            {
                return StatusCode(500, new
                {
                    message =
                        "U appsettings.json nema dovoljno referentnih automobila."
                });
            }

            var existingReferenceIds = await _context.Cars
                .Where(car => referenceCarIds.Contains(car.Id))
                .Select(car => car.Id)
                .ToListAsync();

            if (existingReferenceIds.Count != referenceCarIds.Count)
            {
                var missingIds = referenceCarIds
                    .Except(existingReferenceIds)
                    .ToList();

                return StatusCode(500, new
                {
                    message =
                        "Neki referentni automobili ne postoje u tablici Cars.",
                    missingCarIds = missingIds
                });
            }

            // Svakom korisniku biramo samo 3 iz skupa postojećih 22.
            var selectedReferenceCarIds = existingReferenceIds
                .OrderBy(_ => Guid.NewGuid())
                .Take(referenceCarsPerUser)
                .ToList();

            /*
             * Ostalih 27 biramo među automobilima izvan stara 22.
             * Prednost imaju automobili koji su najmanje dodjeljivani
             * i koji imaju najmanje ocjena.
             */
            var candidateStats = await _context.Cars
                .AsNoTracking()
                .Where(car => !referenceCarIds.Contains(car.Id))
                .Select(car => new
                {
                    CarId = car.Id,

                    AssignmentCount = _context.SurveyCarAssignments
                        .Count(assignment => assignment.CarId == car.Id),

                    RatingCount = _context.SurveyRatings
                        .Count(rating => rating.CarId == car.Id)
                })
                .ToListAsync();

            if (candidateStats.Count < dynamicCarsPerUser)
            {
                return BadRequest(
                    $"U bazi nema dovoljno automobila za odabir " +
                    $"{dynamicCarsPerUser} promjenjivih automobila."
                );
            }

            var dynamicCarIds = candidateStats
                .OrderBy(x => x.AssignmentCount)
                .ThenBy(x => x.RatingCount)
                .ThenBy(_ => Guid.NewGuid())
                .Take(dynamicCarsPerUser)
                .Select(x => x.CarId)
                .ToList();

            // Ukupno: 3 + 27 = 30.
            var selectedCarIds = selectedReferenceCarIds
                .Concat(dynamicCarIds)
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            var assignments = selectedCarIds.Select(carId =>
                new SurveyCarAssignment
                {
                    SurveyUserId = surveyUserId,
                    CarId = carId,
                    AssignedAt = DateTime.UtcNow
                }
            );

            await _context.SurveyCarAssignments
                .AddRangeAsync(assignments);

            await _context.SaveChangesAsync();

            var selectedCars = await _context.Cars
                .AsNoTracking()
                .Where(car => selectedCarIds.Contains(car.Id))
                .ToListAsync();

            var selectedOrder = selectedCarIds
                .Select((id, index) => new { id, index })
                .ToDictionary(x => x.id, x => x.index);

            var orderedSelectedCars = selectedCars
                .OrderBy(car => selectedOrder[car.Id])
                .ToList();

            return Ok(orderedSelectedCars);
        }

        // POST: api/SurveyRating
        [HttpPost]
        public async Task<IActionResult> PostSurveyRating(
            [FromBody] SurveyRating rating)
        {
            if (rating.SurveyUserId <= 0)
            {
                return BadRequest("Neispravan korisnik ankete.");
            }

            if (rating.CarId <= 0)
            {
                return BadRequest("Neispravan automobil.");
            }

            if (rating.Score < 1 || rating.Score > 10)
            {
                return BadRequest("Ocjena mora biti između 1 i 10.");
            }

            var assignedCarIds = await _context.SurveyCarAssignments
                .Where(x => x.SurveyUserId == rating.SurveyUserId)
                .Select(x => x.CarId)
                .ToListAsync();

            /*
             * Ako korisnik ima novi skup od 30 automobila,
             * može ocijeniti samo automobile iz tog skupa.
             */
            if (assignedCarIds.Count > 0 &&
                !assignedCarIds.Contains(rating.CarId))
            {
                return BadRequest(
                    "Ovaj automobil nije dodijeljen korisniku u anketi."
                );
            }

            var existingRating = await _context.SurveyRatings
                .FirstOrDefaultAsync(x =>
                    x.SurveyUserId == rating.SurveyUserId &&
                    x.CarId == rating.CarId
                );

            if (existingRating != null)
            {
                existingRating.Score = rating.Score;
            }
            else
            {
                rating.CreatedAt = DateTime.UtcNow;
                await _context.SurveyRatings.AddAsync(rating);
            }

            await _context.SaveChangesAsync();

            var ratedCount = assignedCarIds.Count > 0
                ? await _context.SurveyRatings
                    .Where(x =>
                        x.SurveyUserId == rating.SurveyUserId &&
                        assignedCarIds.Contains(x.CarId))
                    .Select(x => x.CarId)
                    .Distinct()
                    .CountAsync()
                : await _context.SurveyRatings
                    .Where(x => x.SurveyUserId == rating.SurveyUserId)
                    .Select(x => x.CarId)
                    .Distinct()
                    .CountAsync();

            return Ok(new
            {
                message = "Ocjena je spremljena.",
                rating = existingRating ?? rating,
                ratedCount,
                requiredRatings = 30,
                completed = ratedCount >= 30
            });
        }
    }
}