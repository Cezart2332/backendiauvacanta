using Microsoft.AspNetCore.Mvc;
using IauVacanta.Backend.DTOs;
using IauVacanta.Backend.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace IauVacanta.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlaceController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PlaceDto>>> GetPlaces([FromQuery] string? city, [FromQuery] int? stars, [FromQuery] List<string>? facilities)
        {
            var places = await _placeService.Search(new PlaceFilterDto
            {
                City = city,
                Stars = stars,
                Facilities = facilities
            });

            return Ok(places);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<PlaceDto>> CreatePlace(CreatePlaceRequestDto placeDto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized();
            }

            var place = await _placeService.Create(int.Parse(userIdStr), placeDto);
            if (place == null)
            {
                return BadRequest("Failed to create place.");
            }

            return CreatedAtAction(nameof(GetPlaces), new { id = place.Id }, place);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("pending")]
        public async Task<ActionResult<List<PlaceDto>>> GetPending()
        {
            var pending = await _placeService.GetPending();
            return Ok(pending);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPatch("{id:int}/approval")]
        public async Task<ActionResult<PlaceDto>> SetApproval(int id, [FromQuery] bool approved = true)
        {
            var place = await _placeService.Approve(id, approved);
            if (place == null)
            {
                return NotFound();
            }

            return Ok(place);
        }
    }
}