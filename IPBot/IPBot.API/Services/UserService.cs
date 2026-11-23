using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using IPBot.API.Domain.Entities;
using IPBot.API.Domain.Interfaces;
using IPBot.API.Domain.Utilities;
using IPBot.Common.Dtos;
using IPBot.Common.Services;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.API.Services;

internal class UserService(IConfiguration configuration, IUserRepository userRepository) : IUserService
{
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();
    private readonly IConfiguration _configuration = configuration;
    private readonly IUserRepository _userRepository = userRepository;
    private SigningCredentials _signingCredentials;

    public async Task<bool> RegisterUserAsync(UserDto dto)
    {
        PasswordUtility.CreatePasswordHash(dto.Password, out var passwordHash, out var passwordSalt);

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        return await _userRepository.AddAsync(user);
    }

    public async Task<string> LoginUserAsync(UserDto dto)
    {
        var user = await _userRepository.GetWhereAsync(x => x.Username == dto.Username);

        return user is not null && VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt)
            ? CreateToken(user)
            : string.Empty;
    }

    private static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);

        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        return computedHash.SequenceEqual(passwordHash);
    }

    private string CreateToken(User user)
    {
        _signingCredentials ??= CreateSigningCredentials();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: _signingCredentials);

        var jwt = _jwtSecurityTokenHandler.WriteToken(token);

        return jwt;
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var securityKeyToken = _configuration["SecurityKeyToken"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKeyToken));

        return new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
    }
}
