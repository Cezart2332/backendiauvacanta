using IauVacanta.Backend.DTOs;

namespace IauVacanta.Backend.Interfaces
{
    public interface IPlaceService
    {
        Task<List<PlaceDto>> Search(PlaceFilterDto filter);
        Task<PlaceDto?> Create(int ownerId, CreatePlaceRequestDto request);
        Task<PlaceDto?> Approve(int placeId, bool isApproved);
        Task<List<PlaceDto>> GetPending();
        Task<PlaceDto?> Update(int placeId, int actorUserId, bool isAdmin, CreatePlaceRequestDto request);
        Task<bool> Delete(int placeId, int actorUserId, bool isAdmin);
    }
}
