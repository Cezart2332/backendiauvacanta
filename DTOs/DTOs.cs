using System.ComponentModel.DataAnnotations;

namespace IauVacanta.Backend.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(120)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class ProfileDto
    {
        public string Description { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public ProfileDto? Profile { get; set; }
    }

    public class AuthResponseDto
    {
        public UserDto User { get; set; } = new();
    }

    public class PlaceDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PhotoUrl { get; set; } = string.Empty;
        public int Stars { get; set; }
        public string City { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public int OwnerId { get; set; }
        public List<string> Facilities { get; set; } = new();
    }

    public class PlaceFilterDto
    {
        public string? City { get; set; }
        [Range(1, 5)]
        public int? Stars { get; set; }
        public List<string>? Facilities { get; set; }
    }

    public class CreatePlaceRequestDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(10)]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Url]
        public string PhotoUrl { get; set; } = string.Empty;

        [Range(1, 5)]
        public int Stars { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(120)]
        public string City { get; set; } = string.Empty;

        public List<string> Facilities { get; set; } = new();
    }

    public class CreateReservationRequestDto
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(1, int.MaxValue)]
        public int PlaceId { get; set; }
    }

    public class ReservationDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PlaceId { get; set; }
        public string PlaceTitle { get; set; } = string.Empty;
        public int UserId { get; set; }
    }

    public class UpdateProfileRequestDto
    {
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Url]
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}