using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingApp.API.Helpers;
using MessagingApp.API.Models;

namespace MessagingApp.API.Data
{
    public interface IMessagingRepository
    {
        void Add<T>(T entity) where T: class;  //Instead of specifying two methods for adding User and adding Photo, we use a generic Add<T> method
        void Delete<T>(T entity) where T: class;

        Task<bool> SaveAll();
        Task<PagedList<User>> GetUsers(UserParams userParams);
        Task<User> GetUser(int id);

        Task<Photo> GetPhoto(int id);

        Task<Photo> GetMainPhotoForUser(int userId);

        Task<Message> GetMessage(int id);

        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);

        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
        
    }
}