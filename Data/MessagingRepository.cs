using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MessagingApp.API.Data
{
    public class MessagingRepository : IMessagingRepository
    {
        private readonly DataContext _context;

        public MessagingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        public async Task<User> GetUser(int id)
        {
            var users = await _context.Users.Include(p=> p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return users;
        }
        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users.Include(p=> p.Photos).ToListAsync();       //Since we want to display photos when we display users, we need to include it. EF will automatically return photos related to the user by using Primary Keys and Foreign Keys. 
            return users;
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;       //If nothing's been saved, the SaveChangesAsync() returns 0, and false will be returned. If something's been saved, it returns 1, and since 1>0, the method returns True. 
        }
    }
}