using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PupDate.API.helpers;
using PupDate.API.Models;

namespace PupDate.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
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
        {   // 
            return await _context.Photos.Where(u => u.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {  // returns the first or default that matches the id of the user that I pass in.
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParameters userParameters)
        {
            var users = _context.Users.Include(p => p.Photos).AsQueryable();

            users = users.Where(user => user.Id != userParameters.UserId);
            // filter for gender
            users = users.Where(user => user.Gender == userParameters.Gender);
            // check for min and max age
            if (userParameters.MinimumAge != 18 || userParameters.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParameters.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParameters.MinimumAge);

                users = users.Where(user => user.DateOfBirth >= minDob && user.DateOfBirth <= maxDob);
            }

            return await PagedList<User>.CreateAsync(users, userParameters.PageNumber, userParameters.PageSize);
        }


        public Task<IEnumerable<User>> GetUsers()
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}