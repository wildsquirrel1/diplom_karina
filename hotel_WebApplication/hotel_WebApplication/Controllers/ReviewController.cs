using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly HoteldContext _context;
        private readonly BadWordsFilter  _badWordsFilter;

        public ReviewController(HoteldContext context, BadWordsFilter badWordsFilter)
        {
            _context = context;
            _badWordsFilter = badWordsFilter;
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetReviewsByHotel(int hotelId)
        {
            var reviews = await _context.CommHotels
                .Where(x => x.IdHotel == hotelId)
                .Include(x => x.IdCommNavigation)
                .Include(x => x.IdClientNavigation)
                .Select(x => new
                {
                    commentId = x.IdComm,
                    text = x.IdCommNavigation.Comment1,
                    stars = x.IdCommNavigation.Stars,
                    date = x.IdCommNavigation.Date.HasValue ? x.IdCommNavigation.Date.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    clientName = $"{x.IdClientNavigation.Lastname} {x.IdClientNavigation.Name}".Trim()
                }).OrderByDescending(x => x.date).ToListAsync();
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Comment comment1, [FromQuery] int clientId, [FromQuery] int hotelId)
        {

            if (_badWordsFilter.ContainsBadWords(comment1.Comment1))
            {
                return BadRequest(new
                {
                    error = "Отзыв содержит недопустимые слова",
                    code = "BAD_WORDS_DETECTED"
                });
            }

            comment1.Date = DateOnly.FromDateTime(DateTime.Now);

            _context.Comments.Add(comment1);
            await _context.SaveChangesAsync();

            _context.CommHotels.Add(new CommHotel
            {
                IdComm = comment1.Idcomment,
                IdClient = clientId,
                IdHotel = hotelId
            });
            await _context.SaveChangesAsync();

            return Ok(comment1);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] Comment comment)
        {
            var existing = await _context.Comments.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Comment1 = comment.Comment1;
            existing.Stars = comment.Stars;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }

}
