using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
public class Seed
{
    public static async Task SeedUsers(DataContext context)
    {
        //if users are present already then return
        if(await context.Users.AnyAsync()) return;

        //read the data from file and load them into database
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
        var options = new JsonSerializerOptions{PropertyNameCaseInsensitive= true};
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
        
        foreach (var user in users)
        {
            user.UserName= user.UserName.ToLower();
            using var hmac = new HMACSHA512();
            user.PasswordHash= hmac.ComputeHash(Encoding.UTF8.GetBytes("Password"));
            user.PasswordSalt=hmac.Key;
            context.Users.Add(user);
        }
        
        await context.SaveChangesAsync();
    }

}
