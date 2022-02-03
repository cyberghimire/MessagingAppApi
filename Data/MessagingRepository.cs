using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessagingApp.API.Helpers;
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

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include(u => u.Sender).ThenInclude(u=> u.Photos)
                                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                                .AsQueryable();
            
            switch(messageParams.MessageContainer){
                case "Inbox":
                    messages = messages.Where(u=> u.RecipientId == messageParams.UserId);       //Filter out the messages out of the retrieved messages based on a condition ReciepientId == UserId i.e. filter out the messages sent to the user.
                                                                                                
                    break;
                
                case "Outbox":
                    messages = messages.Where(u=> u.SenderId == messageParams.UserId);          //Filter out the messages sent by the user based on the condition SenderId == UserId 
                    break;
                
                default:
                    messages = messages.Where(u=> u.RecipientId == messageParams.UserId && u.IsRead == false);
                    break;
            }

            messages = messages.OrderByDescending(m => m.MessageSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
            
        }

        public Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(photo => photo.Id ==id);
            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var users = await _context.Users.Include(p=> p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return users;
        }
        public async Task<PagedList<User>> GetUsers(UserParams userParams)      
        {
            // var users = await _context.Users.Include(p=> p.Photos).ToListAsync();       //Since we want to display photos when we display users, we need to include it. EF will automatically return photos related to the user by using Primary Keys and Foreign Keys. 
            var users = _context.Users.Include(p => p.Photos);
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;                               //If nothing's been saved, the SaveChangesAsync() returns 0, and false will be returned. If something's been saved, it returns 1, and since 1>0, the method returns True. 
        }
    }
}