using Microsoft.AspNetCore.Mvc;
using IauVacanta.Backend.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using IauVacanta.Backend.Interfaces;

namespace IauVacanta.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpPost]
        public async Task<ActionResult<ReservationDto>> Reserve(CreateReservationRequestDto request)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdStr);

            var reservation = await _reservationService.Reserve(userId, request);
            if (reservation == null)
            {
                return BadRequest("Reservation request is invalid or dates overlap with an existing reservation.");
            }

            return Ok(reservation);
        }

        [HttpGet("my-reservations")]
        public async Task<ActionResult<List<ReservationDto>>> GetMyReservations()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdStr);
            var reservations = await _reservationService.GetMyReservations(userId);

            return Ok(reservations);
        }
    }
}