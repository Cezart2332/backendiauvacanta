using IauVacanta.Backend.DTOs;

namespace IauVacanta.Backend.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationDto?> Reserve(int userId, CreateReservationRequestDto request);
        Task<List<ReservationDto>> GetMyReservations(int userId);
    }
}
