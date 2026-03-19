using IauVacanta.Backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IauVacanta.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacilityController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FacilityController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> GetFacilities()
        {
            var facilities = await _context.Facilities
                .AsNoTracking()
                .OrderBy(f => f.Name)
                .Select(f => f.Name)
                .ToListAsync();

            return Ok(facilities);
        }
    }
}
