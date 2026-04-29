using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FloorController : ControllerBase
    {
        private readonly HoteldContext _context;

        public FloorController(HoteldContext context)
        {
            _context = context;
        }

        // GET: api/Floor
        [HttpGet]
        public async Task<ActionResult<List<Floor>>> Get()
        {
            return await _context.Floors.ToListAsync();
        }

        // GET: api/Floor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Floor>> Get(int id)
        {
            var floor = await _context.Floors.FindAsync(id);
            if (floor == null) return NotFound();
            return floor;
        }
    }
}
