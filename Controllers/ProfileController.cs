using System.Security.Claims;
using IauVacanta.Backend.Data;
using IauVacanta.Backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IauVacanta.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        public async Task<ActionResult<ProfileDto>> GetMyProfile()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var profile = await _context.Profiles.AsNoTracking().FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
            {
                return NotFound();
            }

            return Ok(new ProfileDto
            {
                Description = profile.Description,
                ProfilePictureUrl = profile.ProfilePictureUrl
            });
        }

        [HttpPut("me")]
        public async Task<ActionResult<ProfileDto>> UpdateMyProfile(UpdateProfileRequestDto request)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
            if (profile == null)
            {
                return NotFound();
            }

            profile.Description = request.Description.Trim();
            profile.ProfilePictureUrl = request.ProfilePictureUrl.Trim();
            await _context.SaveChangesAsync();

            return Ok(new ProfileDto
            {
                Description = profile.Description,
                ProfilePictureUrl = profile.ProfilePictureUrl
            });
        }
    }
}
