using IauVacanta.Backend.Models;
using IauVacanta.Backend.DTOs;

namespace IauVacanta.Backend.Interfaces
{
    public interface IAuthService
    {
        Task<User?> Register(RegisterRequestDto request);
        Task<User?> ValidateCredentials(LoginRequestDto request);
        Task<RefreshToken> GenerateRefreshToken(int userId);
        Task<User?> GetUserByRefreshToken(string token);
        Task<string> CreateToken(User user);
        Task RevokeRefreshToken(string token);
        UserDto MapUser(User user);
    }
}