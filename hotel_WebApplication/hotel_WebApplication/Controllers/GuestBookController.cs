using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestBookController : ControllerBase
    {
        private readonly HoteldContext _ctx;

        public GuestBookController(HoteldContext ctx) => _ctx = ctx;

        // POST: api/GuestBook - добавить гостя в бронь
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] GuestBook guestBook)
        {
            if (guestBook == null || guestBook.GuestId <= 0 || guestBook.Bookid <= 0)
                return BadRequest("Некорректные данные");

            var booking = await _ctx.Books.FindAsync(guestBook.Bookid);
            if (booking == null)
                return NotFound("Бронирование не найдено");

            var guest = await _ctx.Guests.FindAsync(guestBook.GuestId);
            if (guest == null)
                return NotFound("Гость не найден");

            var exists = await _ctx.GuestBooks
                .AnyAsync(gb => gb.Bookid == guestBook.Bookid && gb.GuestId == guestBook.GuestId);

            if (exists)
                return BadRequest("Этот гость уже добавлен в данное бронирование");

            _ctx.GuestBooks.Add(guestBook);
            await _ctx.SaveChangesAsync();

            return Ok(guestBook);
        }

        // GET: api/GuestBook/booking/5 - получить всех гостей брони
        [HttpGet("booking/{bookingId}")]
        public async Task<IActionResult> GetGuestsByBooking(int bookingId)
        {
            var guests = await _ctx.GuestBooks
                .Where(gb => gb.Bookid == bookingId)
                .Include(gb => gb.Guest)
                .Select(gb => new
                {
                    gb.Guest.Idguest,
                    gb.Guest.Lastname,
                    gb.Guest.Name,
                    gb.Guest.Patronymic,
                    gb.Guest.DocumentType,
                    gb.Guest.DocumentNumber
                })
                .ToListAsync();

            return Ok(guests);
        }
    }
}
