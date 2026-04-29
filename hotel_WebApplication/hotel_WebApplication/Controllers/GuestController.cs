using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
    {
        private readonly HoteldContext _context;

        public GuestController(HoteldContext context)
        {
            _context = context;
        }
        // GET: api/<GuestController>
        [HttpGet]
        public async Task<ActionResult<List<Guest>>> Get()
        {
            return Ok(await _context.Guests
                .ToListAsync());
        }

        // POST api/<GuestController>
        [HttpPost]
        public async Task<ActionResult<Guest>> Post(Guest newGuest)
        {
            _context.Guests.Add(newGuest);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = newGuest.Idguest }, newGuest);
        }
    }
}
