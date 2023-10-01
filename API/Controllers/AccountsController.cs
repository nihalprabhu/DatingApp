using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.DTOs;
using API.Interfaces;

namespace API.Controllers;
public class AccountsController: BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;

    public AccountsController(DataContext context, ITokenService tokenService)
    {
        this._context = context;
        this._tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
    {
        if(await UserExists(registerDto.Username)) return BadRequest("Username already taken.");
        using var hmac = new HMACSHA512();
        var user = new AppUser
        {
            UserName=registerDto.Username.ToLower(),
            PasswordHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt= hmac.Key
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserDTO
        {
            Username= user.UserName,
            Token= _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserName==loginDto.Username);
        if(user ==null) return Unauthorized("User is not present");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computeHash= hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computeHash.Length; i++)
        {
            if(computeHash[i]!= user.PasswordHash[i]) return Unauthorized("Password is incorrect");
        }

        return new UserDTO
        {
            Username= user.UserName,
            Token= _tokenService.CreateToken(user)
        };

    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x=>x.UserName==username.ToLower());
    }
}