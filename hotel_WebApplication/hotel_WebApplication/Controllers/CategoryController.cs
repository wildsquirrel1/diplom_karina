using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly HoteldContext _context;

        public CategoryController(HoteldContext context)
        {
            _context = context;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _context.Categories.Include(c => c.PhotoCategories)
            .ThenInclude(pc => pc.Photo).ToListAsync();

            return Ok(categories);
        }

        // GET: api/Category/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.Include(c => c.PhotoCategories)
            .ThenInclude(pc => pc.Photo).FirstOrDefaultAsync(c => c.Idcategory == id);
            if (category == null)
                return NotFound("Категория не найдена");
            return Ok(category);
        }

        // GET: api/Category/5/rooms
        [HttpGet("{id}/rooms")]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoomsByCategory(int id)
        {
            var rooms = await _context.Rooms
                .Where(r => r.IdCategory == id)
                .Include(r => r.Status)
                .Include(r => r.Floor)
                .ToListAsync();

            return Ok(rooms);
        }
    }
}
