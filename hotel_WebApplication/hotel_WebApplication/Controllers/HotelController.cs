using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {

        private readonly HoteldContext context;

        public HotelController(HoteldContext _context)
        {
            context = _context;
        }

        // GET: api/<HotelController>
        [HttpGet]
        public async Task<ActionResult<List<Hotel>>> Get()
        {
            var hotels = await context.Hotels.Include(h => h.Employees).ToListAsync();

            return Ok(hotels);
        }

        // GET api/<HotelController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hotel>> Get(int id)
        {
            var hotel = await context.Hotels.Where((hotel) => hotel.Idhotel == id).FirstOrDefaultAsync();

            return hotel == null ? NotFound("Отель не найден") : Ok(hotel);
        }

        // POST api/<HotelController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Hotel newHotel)
        {
            if (newHotel == null)
                return BadRequest("Данные отеля не указаны");
            if (context.Hotels.Any(h => h.Name == newHotel.Name))
                return BadRequest("Отель с таким названием уже существует");
            if (context.Hotels.Any(h => h.Email == newHotel.Email))
                return BadRequest("Электронная почта уже используется другим отелем");
            if (context.Hotels.Any(h => h.PhoneNumber == newHotel.PhoneNumber))
                return BadRequest("Номер телефона уже используется другим отелем");

            context.Hotels.Add(newHotel);
            await context.SaveChangesAsync();

            return NoContent();
        }

        // PUT api/<HotelController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Hotel Updatehotel)
        {
            if (Updatehotel == null || Updatehotel.Idhotel != id)
            {
                return BadRequest("Что-то пошло не так. Еще раз проверьте данные");
            }

            var existingWithSameName = context.Hotels.FirstOrDefault(h => h.Name == Updatehotel.Name && h.Idhotel != id);
            if (existingWithSameName != null)
                return BadRequest("Отель с таким названием уже существует");

            var existingWithSameEmail = context.Hotels.FirstOrDefault(h => h.Email == Updatehotel.Email && h.Idhotel != id);
            if (existingWithSameEmail != null)
                return BadRequest("Электронная почта уже используется другим отелем");

            var existingWithSamePhone = context.Hotels.FirstOrDefault(h => h.PhoneNumber == Updatehotel.PhoneNumber && h.Idhotel != id);
            if (existingWithSamePhone != null)
                return BadRequest("Номер телефона уже используется другим отелем");

            var existingHotel = context.Hotels.FirstOrDefault(h => h.Idhotel == id);
            if (existingHotel == null)
                return NotFound("Отель не найден");

            existingHotel.Name = Updatehotel.Name;
            existingHotel.Address = Updatehotel.Address;
            existingHotel.City = Updatehotel.City;
            existingHotel.PhoneNumber = Updatehotel.PhoneNumber;
            existingHotel.Email = Updatehotel.Email;
            existingHotel.Photo = Updatehotel.Photo;

            await context.SaveChangesAsync();
            return NoContent();
        }

        // GET api/<HotelController>/5/stars
        [HttpGet("{id}/stars")]
        public async Task<ActionResult<object>> GetAverageHotelRatingAsync(int id)
        {
            try
            {
                var ratings = await context.CommHotels
                    .Where(ch => ch.IdHotel == id)
                    .Include(ch => ch.IdCommNavigation)
                    .Select(ch => ch.IdCommNavigation.Stars)
                    .ToListAsync();

                if (ratings == null || ratings.Count == 0)
                    return Ok(new { rating = (double?)null, count = 0 });

                return Ok(new
                {
                    rating = Math.Round(ratings.Average(), 1),
                    count = ratings.Count
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка: {ex.Message}");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

    }
}
