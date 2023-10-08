using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;
public class UserRepository : IUserRepository
{
    private readonly DataContext _context;


    public UserRepository(DataContext context)
    {
        _context = context;

    }
    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.Include(p=>p.Photos).FirstOrDefaultAsync<AppUser>(x=>x.UserName==username.ToLower());
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsysnc()
    {
        return await _context.Users
                             .Include(p=>p.Photos)
                             .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync()>0;
    }

    public void Update(AppUser user)
    {
        _context.Entry(user).State=EntityState.Modified;
    }
}
