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
            if (await _context.Guests.AnyAsync(g => g.DocumentNumber == newGuest.DocumentNumber))
            {
                return BadRequest(new { error = "Документ с таким номером уже зарегистрирован" });
            }

            _context.Guests.Add(newGuest);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = newGuest.Idguest }, newGuest);
        }

        // PUT: api/Guest/{id} (редактирование)
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Guest updatedGuest)
        {
            if (id != updatedGuest.Idguest)
                return BadRequest("Идентификатор в пути и в теле запроса не совпадают");

            var existingGuest = await _context.Guests.FindAsync(id);
            if (existingGuest == null)
                return NotFound("Гость не найден");

            if (existingGuest.DocumentNumber != updatedGuest.DocumentNumber)
            {
                if (await _context.Guests.AnyAsync(g => g.DocumentNumber == updatedGuest.DocumentNumber && g.Idguest != id && g.Status != 2))
                {
                    return BadRequest(new { error = "Документ с таким номером уже зарегистрирован" });
                }
            }

            // Обновляем только разрешённые поля
            existingGuest.Name = updatedGuest.Name;
            existingGuest.Lastname = updatedGuest.Lastname;
            existingGuest.Patronymic = updatedGuest.Patronymic;
            existingGuest.DocumentType = updatedGuest.DocumentType;
            existingGuest.DocumentNumber = updatedGuest.DocumentNumber;
            existingGuest.Status = updatedGuest.Status;

            await _context.SaveChangesAsync();
            return NoContent(); // 204 - успешно обновлено
        }
    }
}
