using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hotel_WebApplication.Model;

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClintGuestController : ControllerBase
    {
        private readonly HoteldContext _context;

        public ClintGuestController(HoteldContext context)
        {
            _context = context;
        }

        // GET: api/ClintGuest/client/{clientId}
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<List<Guest>>> GetByClient(int clientId)
        {
            var clientExists = await _context.Clints.AnyAsync(c => c.Idclint == clientId);
            if (!clientExists)
                return NotFound("Клиент не найден");

            var guests = await _context.ClintGuests
                .Where(cg => cg.Clientid == clientId)
                .Include(cg => cg.GuestItNavigation)
                .Select(cg => cg.GuestItNavigation)
                .ToListAsync();

            return Ok(guests);
        }

        // POST: api/ClintGuest
        [HttpPost]
        public async Task<ActionResult<ClintGuest>> PostClintGuest([FromBody] ClintGuest clintGuest)
        {
            // Проверяем, не создана ли уже такая связь
            var exists = await _context.ClintGuests
                .AnyAsync(cg => cg.Clientid == clintGuest.Clientid && cg.GuestIt == clintGuest.GuestIt);

            if (exists)
                return BadRequest("Гость уже привязан к этому клиенту");

            _context.ClintGuests.Add(clintGuest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByClient),
                new { clientId = clintGuest.Clientid }, clintGuest);
        }

        
        // GET: api/ClintGuest/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ClintGuest>> GetClintGuest(int id)
        {
            var clintGuest = await _context.ClintGuests
                .Include(cg => cg.GuestItNavigation)
                .FirstOrDefaultAsync(cg => cg.IdclintGuest == id);

            if (clintGuest == null)
                return NotFound();

            return Ok(clintGuest);
        }
    }
}