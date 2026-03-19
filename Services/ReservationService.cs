using IauVacanta.Backend.Data;
using IauVacanta.Backend.DTOs;
using IauVacanta.Backend.Interfaces;
using IauVacanta.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace IauVacanta.Backend.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;

        public ReservationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ReservationDto?> Reserve(int userId, CreateReservationRequestDto request)
        {
            var start = request.StartDate.Date;
            var end = request.EndDate.Date;

            if (end <= start)
            {
                return null;
            }

            var place = await _context.Places.FirstOrDefaultAsync(p => p.Id == request.PlaceId && p.IsApproved);
            if (place == null)
            {
                return null;
            }

            var hasOverlap = await _context.Reservations
                .AnyAsync(r => r.PlaceId == request.PlaceId && r.StartDate < end && start < r.EndDate);

            if (hasOverlap)
            {
                return null;
            }

            var reservation = new Reservation
            {
                StartDate = start,
                EndDate = end,
                PlaceId = request.PlaceId,
                UserId = userId
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return new ReservationDto
            {
                Id = reservation.Id,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                PlaceId = reservation.PlaceId,
                PlaceTitle = place.Title,
                UserId = reservation.UserId
            };
        }

        public async Task<List<ReservationDto>> GetMyReservations(int userId)
        {
            return await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Place)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.StartDate)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    PlaceId = r.PlaceId,
                    PlaceTitle = r.Place != null ? r.Place.Title : string.Empty,
                    UserId = r.UserId
                })
                .ToListAsync();
        }
    }
}
