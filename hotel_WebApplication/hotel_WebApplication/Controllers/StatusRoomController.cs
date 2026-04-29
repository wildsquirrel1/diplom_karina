using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusRoomController : ControllerBase
    {
        private readonly HoteldContext _context;

        public StatusRoomController(HoteldContext context)
        {
            _context = context;
        }

        // GET: api/StatusRoom
        [HttpGet]
        public async Task<ActionResult<List<StatusRoom>>> Get()
        {
            return await _context.StatusRooms.ToListAsync();
        }

        // GET: api/StatusRoom/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StatusRoom>> Get(int id)
        {
            var status = await _context.StatusRooms.FindAsync(id);
            if (status == null) return NotFound();
            return status;
        }
    }
}
