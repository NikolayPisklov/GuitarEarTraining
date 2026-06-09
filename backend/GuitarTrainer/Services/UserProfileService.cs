using GuitarTrainer.Dtos;
using GuitarTrainer.Model;

namespace GuitarTrainer.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly AppDbContext _db;
        
        public UserProfileService(AppDbContext db) 
        {
            _db = db;
        }

        public async Task<UserNameDto> GetUserFullNameAsync(Guid userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user is null)
            {
                throw new Exception();
            }
            return new UserNameDto(user.FirstName, user.LastName);
        }

        public async Task<bool> IsUserWithNameAsync(Guid userId) 
        {
            var user = await _db.Users.FindAsync(userId);
            if(user is null)
            {
                //Not found
                throw new Exception();
            }
            if(string.IsNullOrEmpty(user.FirstName) && string.IsNullOrEmpty(user.LastName)) 
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public async Task UpdateUserFullNameAsync(Guid userId, string firstName, string lastName)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user is null)
            {
                //Not found
                throw new Exception();
            }
            user.FirstName = firstName;
            user.LastName = lastName;
            await _db.SaveChangesAsync();
        }
    }
}
