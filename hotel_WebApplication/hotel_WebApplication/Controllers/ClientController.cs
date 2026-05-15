using hotel_WebApplication.Model;
using hotel_WebApplication.Model.DTO;  
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        //async Task <> и await надо добавить!!!!
        private readonly HoteldContext _context;

        public ClientController(HoteldContext context)
        {
            _context = context;
        }
        // GET: api/<ClientController>
        [HttpGet]
        public async Task<ActionResult<List<Clint>>> Get()
        {
            return Ok(await _context.Clints
                .ToListAsync());
        }

        // GET api/<ClientController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Clint>> Get(int id)
        {
            var client = await _context.Clints.Include(b => b.Books)
                .FirstOrDefaultAsync(c => c.Idclint == id);
            return client == null ? NotFound() : Ok(client);
        }

        // POST api/<ClientController>
        /* [HttpPost] походу тоже самое что и регистрация!!!! надо проверить позже
         public async Task<ActionResult<Clint>> Post(Clint newClient)
         {
             _context.Clints.Add(newClient);
             await _context.SaveChangesAsync();
             return CreatedAtAction(nameof(Get), new { id = newClient.Idclint }, newClient);
         }*/
        //DTO регистрация надо убрать будет, но не сейчас а с мобилкой когда работать буду!!!!!!!!!!
        [HttpPost("Register")]
        public async Task<ActionResult<Clint>> Register([FromBody] RegisterClientDto dto)
        {
            // Валидация даты
            if (!DateOnly.TryParse(dto.Birth, out var birthDate))
                return BadRequest("Неверный формат даты рождения. Ожидается 'yyyy-MM-dd'.");

            if (await _context.Clints.AnyAsync(c => c.Email == dto.Email))
                return BadRequest("Email уже используется.");

            var today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - birthDate.Year;

            if (birthDate.DayOfYear > today.DayOfYear)
            {
                age--;
            }

            if (age < 18)
                return BadRequest("Регистрация возможна только для лиц старше 18 лет.");

            if (age >= 100)
                return BadRequest("Максимальный возраст для регистрации — 99 лет.");

            var client = new Clint
            {
                Name = dto.Name,
                Lastname = dto.Lastname,
                Patronymic = dto.Patronymic,
                SeriaPass = dto.SeriaPass,
                NumberPass = dto.NumberPass,
                Birth = birthDate,
                Email = dto.Email,
                Password = dto.Password
            };

            _context.Clints.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = client.Idclint }, client);
        }

        // POST: api/Client/Auth
        [HttpPost("Auth")]
        public async Task<ActionResult<Clint>> Auth([FromBody] LoginRequest request)
        {
            var client = await _context.Clints
                .FirstOrDefaultAsync(c => c.Email == request.Email && c.Password == request.Password);

            if (client == null)
                return BadRequest(new { error = "Неверный email или пароль." });

            return Ok(client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RegisterClientDto dto)
        {
            var existingClient = await _context.Clints.FindAsync(id);
            if (existingClient == null)
                return NotFound("Клиент не найден.");

            if (!DateOnly.TryParse(dto.Birth, out var birthDate))
                return BadRequest("Неверный формат даты рождения. Ожидается 'yyyy-MM-dd'.");

            var today = DateOnly.FromDateTime(DateTime.Today);
            int age = today.Year - birthDate.Year;

            if (birthDate.DayOfYear > today.DayOfYear)
            {
                age--;
            }

            if (age < 18)
                return BadRequest("Регистрация возможна только для лиц старше 18 лет.");

            if (age >= 100)
                return BadRequest("Максимальный возраст для регистрации — 99 лет.");
           
            if (existingClient.Email != dto.Email)
            {
                if (await _context.Clints.AnyAsync(c => c.Email == dto.Email))
                    return BadRequest("Email уже используется другим клиентом.");
            }

            existingClient.Name = dto.Name;
            existingClient.Lastname = dto.Lastname;
            existingClient.Patronymic = dto.Patronymic;
            existingClient.SeriaPass = dto.SeriaPass;
            existingClient.NumberPass = dto.NumberPass;
            existingClient.Birth = birthDate;
            existingClient.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                existingClient.Password = dto.Password;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        //Узнать для чего он
        private bool ClintExists(int id) =>
        _context.Clints.Any(e => e.Idclint == id);

        // GET: api/Client/{id}/bookings
        [HttpGet("{id}/bookings")]
        public async Task<ActionResult<List<Book>>> GetClientBookings(int id)
        {
            var clientExists = await _context.Clints.AnyAsync(c => c.Idclint == id);
            if (!clientExists)
                return NotFound("Клиент не найден");

            var bookings = await _context.Books.Where(b => b.ClientId == id).Include(b => b.Room).ThenInclude(r => r.IdCategoryNavigation).Include(b => b.Room).ThenInclude(r => r.Hotel).Include(b => b.Room).ThenInclude(r => r.Floor).Include(b => b.BookServices).ThenInclude(bs => bs.Service).Include(b => b.GuestBooks).OrderByDescending(b => b.Idbook).ToListAsync(); 

            return Ok(bookings);
        }

        [HttpGet("{id}/guests")]
        public async Task<ActionResult<List<Guest>>> GetClientGuests(int id)
        {
            var clientExists = await _context.Clints.AnyAsync(c => c.Idclint == id);
            if (!clientExists)
                return NotFound("Клиент не найден");

            var guests = await _context.ClintGuests
                .Where(cg => cg.Clientid == id)
                .Include(cg => cg.GuestItNavigation)
                .Select(cg => cg.GuestItNavigation)
                .ToListAsync();

            return Ok(guests);
        }
    }
}
