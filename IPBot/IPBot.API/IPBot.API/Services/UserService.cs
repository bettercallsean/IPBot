using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Domain.Utilities;
using IPBot.Shared.Dtos;
using IPBot.Shared.Services;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.API.Services;
public class UserService(IConfiguration configuration, IUserRepository userRepository) : IUserService
{
    public async Task<bool> RegisterUserAsync(UserDto dto)
    {
        PasswordUtility.CreatePasswordHash(dto.Password, out var passwordHash, out var passwordSalt);

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        return await userRepository.AddAsync(user);
    }

    public async Task<string> LoginUserAsync(UserDto dto)
    {
        var user = await userRepository.GetWhereAsync(x => x.Username == dto.Username);

        return user == null || !VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt)
            ? string.Empty
            : CreateToken(user);
    }

    private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);

        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        return computedHash.SequenceEqual(passwordHash);
    }

    private string CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["SecurityKeyToken"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
