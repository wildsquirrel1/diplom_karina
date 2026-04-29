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

        // POST api/<ServiceController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}
