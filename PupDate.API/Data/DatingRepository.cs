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

        public async Task<Like> GetLike(int userId, int recipientId)
        {   
            return await _context.Likes.FirstOrDefaultAsync(user => 
                user.LikerId == userId && user.LikeeId == recipientId);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {   // 
            return await _context.Photos.Where(u => u.UserId == userId)
                .FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Message> GetMessage(int userId)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == userId);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParameters messageParams)
        {
            var messages = _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .AsQueryable();

                switch (messageParams.MessageContainer)
                {
                    case "Inbox":
                            messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                            break;
                        case "Outbox":
                            messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                            break;
                        default:
                            messages = messages.Where(u => u.RecipientId == messageParams.UserId 
                            && u.RecipientDeleted == false && u.IsRead == false);
                            break;
                }
            messages = messages.OrderByDescending(d => d.MessageSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.RecipientId == userId && m.RecipientDeleted == false
                    && m.SenderId == recipientId
                    || m.RecipientId == recipientId && m.SenderId == userId)
                // orders by most recent messages
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync();

            return messages; 
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
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(user => user.LastActive).AsQueryable();

            users = users.Where(user => user.Id != userParameters.UserId);
            // filter for gender
            users = users.Where(user => user.Gender == userParameters.Gender);

            if (userParameters.Likers)
            {   // if any of the likers matches any of the id's in the table, will return those users 
                var userLikers = await GetUserLikes(userParameters.UserId, userParameters.Likers);
                users = users.Where(u => userLikers.Contains(u.Id));
            }

            if (userParameters.Likees)
            {
                var userLikees = await GetUserLikes(userParameters.UserId, userParameters.Likers);
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            // check for min and max age
            if (userParameters.MinimumAge != 18 || userParameters.MaxAge != 99)
            {

                var minDob = DateTime.Today.AddYears(-userParameters.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParameters.MinimumAge);

                users = users.Where(user => user.DateOfBirth >= minDob && user.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(userParameters.OrderBy))
            {
                switch (userParameters.OrderBy){
                    case "created": 
                        users = users.OrderByDescending(u => u.Created);
                        break;
                        default: 
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParameters.PageNumber, userParameters.PageSize);
        }


        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

         //  only return users that contain either the liker iD or likerId thats requsted in the parameters.
        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users
                .Include(x => x.Likers)
                .Include(x => x.Likees)
                .FirstOrDefaultAsync(y => y.Id == id);

                if (likers)
                {
                    return user.Likers.Where(x => x.LikeeId == id)
                        // return a collection of integers
                        .Select(i => i.LikerId);
                }
                else
                {
                    return user.Likees.Where(x => x.LikerId == id).Select(i => i.LikeeId);
                }
        }
    }
}