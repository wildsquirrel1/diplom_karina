using hotel_WebApplication.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace hotel_WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly HoteldContext _context;

        public FavoritesController(HoteldContext context)
        {
            _context = context;
        }
        // Получить избранное для клиента
            [HttpGet("client/{idclint}")]
            public async Task<IActionResult> GetFavorites(int idclint)
            {
                var favorites = await _context.Favorites.Where(f => f.Idclint == idclint).ToListAsync();
                return Ok(favorites);
            }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] Favorite favorite)
        {
            // Проверка на дубликат
            var exists = await _context.Favorites
                .AnyAsync(f => f.Idclint == favorite.Idclint && f.Idcategory == favorite.Idcategory);

            if (exists) return BadRequest("Уже в избранном");

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // Удалить из избранного
        [HttpDelete("{idclint}/{idcategory}")]
        public async Task<IActionResult> RemoveFavorite(int idclint, int idcategory)
        {
            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.Idclint == idclint && f.Idcategory == idcategory);

            if (favorite == null) return NotFound();

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
