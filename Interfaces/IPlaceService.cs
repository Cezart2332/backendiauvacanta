using IauVacanta.Backend.DTOs;

namespace IauVacanta.Backend.Interfaces
{
    public interface IPlaceService
    {
        Task<List<PlaceDto>> Search(PlaceFilterDto filter);
        Task<PlaceDto?> Create(int ownerId, CreatePlaceRequestDto request);
        Task<PlaceDto?> Approve(int placeId, bool isApproved);
        Task<List<PlaceDto>> GetPending();
    }
}
