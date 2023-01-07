using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using IPBot.DataServices.Dtos;
using IPBot.DataServices.Interfaces.DataServices;
using IPBot.DataServices.Interfaces.Services;
using IPBot.DataServices.Models;
using IPBot.DataServices.Utilities;
using IPBot.Infrastructure.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace IPBot.DataServices.Service;
public class UserService : IUserService
{
    private readonly IUserDataService _userDataService;

    public UserService(IUserDataService userDataService)
    {
        _userDataService = userDataService;
    }

    public async Task<User> RegisterUserAsync(UserDto dto)
    {
        PasswordUtility.CreatePasswordHash(dto.Password, out var passwordHash, out var passwordSalt);

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt
        };

        await _userDataService.CreateAsync(user);

        return user;
    }

    public async Task<string> LoginUserAsync(UserDto dto)
    {
        var user = await _userDataService.GetByUsernameAsync(dto.Username);

        return user == null || !VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt)
            ? string.Empty
            : CreateToken(user);
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
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

        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(DotEnvHelper.EnvironmentVariables["SECURITY_KEY_TOKEN"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }
}
