using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly HoteldContext context;

        public BookingController(HoteldContext _context)
        {
            context = _context;
        }

        [HttpGet("hotel/{hotelId}/period")]
        public async Task<ActionResult<List<Book>>> GetByHotelAndPeriod(int? hotelId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            //как-то добавить гостей клиентов
            var query = context.Books
        .Include(b => b.Room).ThenInclude(r => r.IdCategoryNavigation).
        Include(b => b.Client).Include(b => b.GuestBooks).ThenInclude(gb => gb.Guest).Where(b => b.Room.Hotelid == hotelId);

            if (startDate.HasValue)
                query = query.Where(b => b.CheckInDate >= DateOnly.FromDateTime(startDate.Value));
            if (endDate.HasValue)
                query = query.Where(b => b.DepartureDate <= DateOnly.FromDateTime(endDate.Value));

            return Ok(await query.ToListAsync());
        }

        // GET api/<BookingController>/5
        [HttpGet("hotel/current")]
        public async Task<ActionResult<List<Book>>> GetBookingsForCurrentHotel([FromQuery] int employeeId)
        {
            var employee = await context.Employees.FirstOrDefaultAsync(e => e.Idemployee == employeeId);

            if (employee == null || employee.Idhotel == null)
                return BadRequest("Сотрудник не привязан к отелю");

            var books = await context.Books.Include(b => b.Room).ThenInclude(r => r.IdCategoryNavigation).Include(b => b.Client).Include(b => b.GuestBooks).ThenInclude(gb => gb.Guest).Include(b => b.BookServices).ThenInclude(bs => bs.Service).Where(b => b.Room.Hotelid == employee.Idhotel).ToListAsync();

            return Ok(books);
        }

        [HttpGet("room/{roomId}/availability")]
        public async Task<ActionResult<object>> CheckRoomAvailability(int RoomId, [FromQuery] DateOnly checkin, [FromQuery] DateOnly checkout)
        {
            var room = await context.Rooms.FindAsync(RoomId);
            if (room == null)
                return NotFound("Номер не найден");

            var isBusy = await context.Books.AnyAsync(b =>
                b.RoomId == RoomId &&
                b.StatusBook != 2 &&
                b.CheckInDate < checkout &&  
                b.DepartureDate > checkin);

            return Ok(new
            {
                roomId = RoomId,
                isAvailable = !isBusy,
                checkIn = checkin,
                checkOut = checkout,
                message = isBusy
                    ? "Номер уже забронирован на этот период"
                    : "Номер свободен"
            });
        }
        //эти два метода очень схожи!!!
        // POST api/<BookingController>
        [HttpPost]
        public async Task<ActionResult<Book>> Post([FromBody] Book newBooking)
        {
            if (newBooking == null)
                return BadRequest("Бронирование не может быть пустым");

            var client = await context.Clints.FindAsync(newBooking.ClientId);
            if (client == null)
                return BadRequest("Клиент не найден");

            newBooking.Client = client;
            newBooking.BookingDate = DateOnly.FromDateTime(DateTime.Now);

            var conflict = context.Books.Any(b =>
                b.RoomId == newBooking.RoomId &&
                b.StatusBook != 2 &&
                b.CheckInDate < newBooking.DepartureDate &&
                b.DepartureDate > newBooking.CheckInDate);

            if (conflict)
                return BadRequest("Номер уже забронирован на этот период");

            context.Books.Add(newBooking);
            await context.SaveChangesAsync();

            return Ok(newBooking);
        }

        // PUT api/<BookingController>/5
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatusOnly(int id, [FromQuery] sbyte status)
        {
            var existing = await context.Books.FindAsync(id);
            if (existing == null) return NotFound();

            existing.StatusBook = status;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("room/{roomId}/busy-dates")]
        public async Task<ActionResult<List<object>>> GetBusyDates(int roomId, [FromQuery] int monthsAhead = 3)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var maxDate = today.AddMonths(monthsAhead);

            var busyPeriods = await context.Books
                .Where(b => b.RoomId == roomId &&
                            b.StatusBook != 2 &&
                            b.CheckInDate <= maxDate)
                .Select(b => new
                {
                    start = b.CheckInDate,
                    end = b.DepartureDate
                })
                .ToListAsync();

            return Ok(busyPeriods);
        }

    }
}
