using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly HoteldContext _context;

        public EmployeeController(HoteldContext context)
        {
            _context = context;
        }


        // GET: api/<EmployeeController>
        [HttpGet]
        public async Task<ActionResult<List<Employee>>> Get()
        {
            return Ok(await _context.Employees.ToListAsync());
        }

        // GET: api/Employee/5
        [HttpGet("{email}, {password}")]
        public async Task<ActionResult<Employee>> Get(string email, string password)
        {
            var employee = await _context.Employees.Include(e => e.IdroleNavigation).Include(e => e.IdhotelNavigation).FirstOrDefaultAsync(e => e.Email == email && e.Password == password);

            return employee == null ? NotFound("Сотрудник не найден") : Ok(new { employee.Idemployee, employee.Name, employee.Lastname, employee.Idrole, employee.Email, employee.Password, employee.Status, employee.IdroleNavigation, employee.IdhotelNavigation });
        }

        [HttpGet("for-user/{currentEmployeeId}")]
        public async Task<ActionResult<List<Employee>>> GetEmployeesForCurrentUser(int currentEmployeeId)
        {
            var currentUser =  await _context.Employees
                .Include(e => e.IdroleNavigation)
                .Include(e => e.IdhotelNavigation)
                .FirstOrDefaultAsync(e => e.Idemployee == currentEmployeeId);

            if (currentUser == null)
                return BadRequest("Текущий пользователь не найден");
            List<Employee> result;


            if (currentUser.Idrole == 1)
            {
                result = await _context.Employees
                    .Include(e => e.IdroleNavigation)
                    .Include(e => e.IdhotelNavigation)
                    .Where(e => e.Idrole == 2)
                    .OrderByDescending(e => e.Idemployee)
                    .ToListAsync();
            }
            else if (currentUser.Idrole == 2)
            {
                result = await _context.Employees
                    .Include(e => e.IdroleNavigation)
                    .Include(e => e.IdhotelNavigation)
                    .Where(e => e.Idrole == 3 && e.Idhotel == currentUser.Idhotel)
                    .OrderByDescending(e => e.Idemployee)
                    .ToListAsync();
            }
            else
            {
                result = new List<Employee>();
            }


            return Ok(result);
        }

        // POST api/<EmployeeController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Employee newEmployee)
        {
            if (newEmployee == null)
                return BadRequest("Данные сотрудника не указаны");

            if (_context.Employees.Any(e => e.Email == newEmployee.Email))
                return BadRequest("Электронная почта уже используется другим сотрудником");

            if (_context.Employees.Any(e => e.PhoneNumber == newEmployee.PhoneNumber))
                return BadRequest("Номер телефона уже используется другим сотрудником");

            newEmployee.Status = 0;

            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = newEmployee.Idemployee }, newEmployee);
        }

        // PUT api/<EmployeeController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee updatedEmployee)
        {
            if (updatedEmployee == null || updatedEmployee.Idemployee != id)
                return BadRequest("Некорректные данные");

            var existing = _context.Employees.FirstOrDefault(e => e.Idemployee == id);
            if (existing == null)
                return NotFound("Сотрудник не найден");

            if (_context.Employees.Any(e => e.Email == updatedEmployee.Email && e.Idemployee != id))
                return BadRequest("Электронная почта уже используется другим сотрудником");

            if (_context.Employees.Any(e => e.PhoneNumber == updatedEmployee.PhoneNumber && e.Idemployee != id))
                return BadRequest("Номер телефона уже используется другим сотрудником");

            existing.Name = updatedEmployee.Name;
            existing.Lastname = updatedEmployee.Lastname;
            existing.Patronymic = updatedEmployee.Patronymic;
            existing.Email = updatedEmployee.Email;
            existing.PhoneNumber = updatedEmployee.PhoneNumber;
            existing.Password = updatedEmployee.Password;
            existing.Birth = updatedEmployee.Birth;
            existing.Idrole = updatedEmployee.Idrole;
            existing.Idhotel = updatedEmployee.Idhotel;
            existing.Status = updatedEmployee.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
