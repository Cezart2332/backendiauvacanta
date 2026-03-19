using IauVacanta.Backend.Data;
using IauVacanta.Backend.DTOs;
using IauVacanta.Backend.Interfaces;
using IauVacanta.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace IauVacanta.Backend.Services
{
    public class PlaceService : IPlaceService
    {
        private readonly AppDbContext _context;

        public PlaceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<PlaceDto>> Search(PlaceFilterDto filter)
        {
            var query = _context.Places
                .AsNoTracking()
                .Include(p => p.Facilities)
                .Where(p => p.IsApproved)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.City))
            {
                var city = filter.City.Trim().ToLower();
                query = query.Where(p => p.City.ToLower() == city);
            }

            if (filter.Stars.HasValue)
            {
                query = query.Where(p => p.Stars == filter.Stars.Value);
            }

            if (filter.Facilities != null && filter.Facilities.Count > 0)
            {
                var normalizedFacilities = filter.Facilities
                    .Where(f => !string.IsNullOrWhiteSpace(f))
                    .Select(f => f.Trim().ToLower())
                    .Distinct()
                    .ToList();

                if (normalizedFacilities.Count > 0)
                {
                    query = query.Where(p => normalizedFacilities.All(required =>
                        p.Facilities.Any(existing => existing.Name.ToLower() == required)));
                }
            }

            return await query
                .Select(p => new PlaceDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    PhotoUrl = p.PhotoUrl,
                    Stars = p.Stars,
                    City = p.City,
                    IsApproved = p.IsApproved,
                    OwnerId = p.OwnerId,
                    Facilities = p.Facilities.Select(f => f.Name).OrderBy(name => name).ToList()
                })
                .ToListAsync();
        }

        public async Task<PlaceDto?> Create(int ownerId, CreatePlaceRequestDto request)
        {
            var facilityNames = request.Facilities
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .Select(f => f.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var place = new Place
            {
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                PhotoUrl = request.PhotoUrl.Trim(),
                Stars = request.Stars,
                City = request.City.Trim(),
                OwnerId = ownerId,
                IsApproved = false
            };

            foreach (var facilityName in facilityNames)
            {
                var lowerName = facilityName.ToLower();
                var facility = await _context.Facilities.FirstOrDefaultAsync(f => f.Name.ToLower() == lowerName);
                if (facility == null)
                {
                    facility = new Facility { Name = facilityName };
                    _context.Facilities.Add(facility);
                }

                place.Facilities.Add(facility);
            }

            _context.Places.Add(place);
            await _context.SaveChangesAsync();

            return new PlaceDto
            {
                Id = place.Id,
                Title = place.Title,
                Description = place.Description,
                PhotoUrl = place.PhotoUrl,
                Stars = place.Stars,
                City = place.City,
                IsApproved = place.IsApproved,
                OwnerId = place.OwnerId,
                Facilities = place.Facilities.Select(f => f.Name).OrderBy(name => name).ToList()
            };
        }

        public async Task<PlaceDto?> Approve(int placeId, bool isApproved)
        {
            var place = await _context.Places.Include(p => p.Facilities).FirstOrDefaultAsync(p => p.Id == placeId);
            if (place == null)
            {
                return null;
            }

            place.IsApproved = isApproved;
            await _context.SaveChangesAsync();

            return new PlaceDto
            {
                Id = place.Id,
                Title = place.Title,
                Description = place.Description,
                PhotoUrl = place.PhotoUrl,
                Stars = place.Stars,
                City = place.City,
                IsApproved = place.IsApproved,
                OwnerId = place.OwnerId,
                Facilities = place.Facilities.Select(f => f.Name).OrderBy(name => name).ToList()
            };
        }

        public async Task<List<PlaceDto>> GetPending()
        {
            return await _context.Places
                .AsNoTracking()
                .Include(p => p.Facilities)
                .Where(p => !p.IsApproved)
                .OrderByDescending(p => p.Id)
                .Select(p => new PlaceDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    PhotoUrl = p.PhotoUrl,
                    Stars = p.Stars,
                    City = p.City,
                    IsApproved = p.IsApproved,
                    OwnerId = p.OwnerId,
                    Facilities = p.Facilities.Select(f => f.Name).OrderBy(name => name).ToList()
                })
                .ToListAsync();
        }

        public async Task<PlaceDto?> Update(int placeId, int actorUserId, bool isAdmin, CreatePlaceRequestDto request)
        {
            var place = await _context.Places
                .Include(p => p.Facilities)
                .FirstOrDefaultAsync(p => p.Id == placeId);

            if (place == null)
            {
                return null;
            }

            if (!isAdmin && place.OwnerId != actorUserId)
            {
                return null;
            }

            place.Title = request.Title.Trim();
            place.Description = request.Description.Trim();
            place.PhotoUrl = request.PhotoUrl.Trim();
            place.Stars = request.Stars;
            place.City = request.City.Trim();
            place.IsApproved = false;

            place.Facilities.Clear();

            var facilityNames = request.Facilities
                .Where(f => !string.IsNullOrWhiteSpace(f))
                .Select(f => f.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var facilityName in facilityNames)
            {
                var lowerName = facilityName.ToLower();
                var facility = await _context.Facilities.FirstOrDefaultAsync(f => f.Name.ToLower() == lowerName);
                if (facility == null)
                {
                    facility = new Facility { Name = facilityName };
                    _context.Facilities.Add(facility);
                }

                place.Facilities.Add(facility);
            }

            await _context.SaveChangesAsync();

            return new PlaceDto
            {
                Id = place.Id,
                Title = place.Title,
                Description = place.Description,
                PhotoUrl = place.PhotoUrl,
                Stars = place.Stars,
                City = place.City,
                IsApproved = place.IsApproved,
                OwnerId = place.OwnerId,
                Facilities = place.Facilities.Select(f => f.Name).OrderBy(name => name).ToList()
            };
        }

        public async Task<bool> Delete(int placeId, int actorUserId, bool isAdmin)
        {
            var place = await _context.Places.FirstOrDefaultAsync(p => p.Id == placeId);
            if (place == null)
            {
                return false;
            }

            if (!isAdmin && place.OwnerId != actorUserId)
            {
                return false;
            }

            _context.Places.Remove(place);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
