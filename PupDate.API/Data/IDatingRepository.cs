using System.Collections.Generic;
using System.Threading.Tasks;
using PupDate.API.helpers;
using PupDate.API.Models;

namespace PupDate.API.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();
         Task<User> GetUser(int id);

        Task<PagedList<User>> GetUsers(UserParameters userParameters);

        Task<Photo> GetPhoto(int id);

        Task<Photo> GetMainPhotoForUser(int id);

        Task<Like> GetLike(int userId, int recipientId);
         
    }
}