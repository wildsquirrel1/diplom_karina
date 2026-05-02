using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly HoteldContext _context;

        public ServiceController(HoteldContext context)
        {
            _context = context;
        }
        // GET: api/<ServiceController>
        [HttpGet]
        public async Task<ActionResult<List<Service>>> Get()
        {
            return Ok(await _context.Services.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Service>> Get(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound("Услуга не найдена");

            return Ok(service);
        }
        // POST api/<ServiceController>
        [HttpPost]
        public async Task<ActionResult<Service>> Post([FromBody] Service newService)
        {
            if (newService == null)
                return BadRequest("Данные услуги не указаны");

            if (_context.Services.Any(s => s.Name == newService.Name))
                return BadRequest("Услуга с таким названием уже существует");

            if (newService.Cost <= 0)
                return BadRequest("Стоимость должна быть больше нуля");

            if (newService.Status == 0)
                newService.Status = 1;

            _context.Services.Add(newService);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = newService.Idservice }, newService);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Service updatedService)
        {
            if (updatedService == null || updatedService.Idservice != id)
                return BadRequest("Некорректные данные");

            var existingService = await _context.Services.FindAsync(id);
            if (existingService == null)
                return NotFound("Услуга не найдена");

            if (_context.Services.Any(s => s.Name == updatedService.Name && s.Idservice != id))
                return BadRequest("Услуга с таким названием уже существует");

            if (updatedService.Cost <= 0)
                return BadRequest("Стоимость должна быть больше нуля");

            existingService.Name = updatedService.Name;
            existingService.Description = updatedService.Description;
            existingService.Cost = updatedService.Cost;
            existingService.Status = updatedService.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
