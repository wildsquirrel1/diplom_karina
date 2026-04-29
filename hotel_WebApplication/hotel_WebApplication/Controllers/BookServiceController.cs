using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookServiceController : ControllerBase
    {
        private readonly HoteldContext _context;

        public BookServiceController(HoteldContext context)
        {
            _context = context;
        }
        // GET: api/<BookServiceController>
        [HttpGet]
        public async Task<ActionResult<List<BookService>>> Get()
        {
            return Ok(await _context.BookServices
                .Include(bs => bs.Service)
                .Include(bs => bs.Book)
                .ToListAsync());
        }

        // POST api/<BookServiceController>
        [HttpPost]
        public async Task<IActionResult> Post(BookService bookService)
        {
            if (bookService == null) return BadRequest();
            _context.BookServices.Add(bookService);
            await _context.SaveChangesAsync();
            return Ok(bookService);
        }
        
    }
}
