using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingApp.API.Models;

namespace MessagingApp.API.Data
{
    public interface IMessagingRepository
    {
        void Add<T>(T entity) where T: class;  //Instead of specifying two methods for adding User and adding Photo, we use a generic Add<T> method
        void Delete<T>(T entity) where T: class;

        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUser(int id);

        
    }
}