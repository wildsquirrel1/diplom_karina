using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {

        private readonly HoteldContext _context;

        public RoomController(HoteldContext context)
        {
            _context = context;
        }
        // GET: api/<RoomController>
        [HttpGet]
        public async Task<ActionResult<List<Room>>> Get()
        {
            return Ok(await _context.Rooms.Include(r => r.IdCategoryNavigation).ThenInclude(c => c.PhotoCategories).ThenInclude(pc => pc.Photo).Include(r => r.Floor).Include(r => r.Hotel).Include(r => r.Status).ToListAsync());
        }

        // GET api/<RoomController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> Get(int id)
        {
            var room = await _context.Rooms.Include(r => r.IdCategoryNavigation).ThenInclude(c => c.PhotoCategories).ThenInclude(pc => pc.Photo).Include(r => r.Floor).Include(r => r.Hotel).Include(r => r.Status).FirstOrDefaultAsync(r => r.Idroom == id);

            if (room == null)
                return NotFound("Номер не найден");

            return Ok(room);
        }

        // POST api/<RoomController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Room newRoom)
        {
            if (newRoom == null)
                return BadRequest("Данные номера не указаны");

            // Проверка: название не должно повторяться в рамках отеля
            if (_context.Rooms.Any(r => r.Name == newRoom.Name && r.Hotelid == newRoom.Hotelid))
                return BadRequest("Номер с таким названием уже существует в этом отеле");

            // Проверка существования связанных сущностей
            if (!_context.Hotels.Any(h => h.Idhotel == newRoom.Hotelid))
                return BadRequest("Указанный отель не существует");
            if (!_context.Categories.Any(c => c.Idcategory == newRoom.IdCategory))
                return BadRequest("Указанная категория не существует");
            if (!_context.Floors.Any(f => f.Idfloor == newRoom.Floorid))
                return BadRequest("Указанный этаж не существует");
            if (!_context.StatusRooms.Any(s => s.IdstatusRoom == newRoom.StatusId))
                return BadRequest("Указанный статус не существует");

            _context.Rooms.Add(newRoom);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = newRoom.Idroom }, newRoom);
        }

        // PUT api/<RoomController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Room updatedRoom)
        {
            if (updatedRoom == null || updatedRoom.Idroom != id)
                return BadRequest("Некорректные данные");

            var existingRoom = _context.Rooms.FirstOrDefault(r => r.Idroom == id);

            if (!_context.Hotels.Any(h => h.Idhotel == updatedRoom.Hotelid))
                return BadRequest("Указанный отель не существует");

            if (existingRoom == null)
                return NotFound("Номер не найден");

            if (_context.Rooms.Any(r => r.Name == updatedRoom.Name && r.Hotelid == updatedRoom.Hotelid && r.Idroom != id))
                return BadRequest("Номер с таким названием уже существует в этом отеле");

            if (!_context.Categories.Any(c => c.Idcategory == updatedRoom.IdCategory))
                return BadRequest("Указанная категория не существует");

            if (!_context.Floors.Any(f => f.Idfloor == updatedRoom.Floorid))
                return BadRequest("Указанный этаж не существует");

            if (!_context.StatusRooms.Any(s => s.IdstatusRoom == updatedRoom.StatusId))
                return BadRequest("Указанный статус не существует");

            existingRoom.Name = updatedRoom.Name;
            existingRoom.Floorid = updatedRoom.Floorid;
            existingRoom.IdCategory = updatedRoom.IdCategory;
            existingRoom.StatusId = updatedRoom.StatusId;
            existingRoom.Hotelid = updatedRoom.Hotelid;

            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
