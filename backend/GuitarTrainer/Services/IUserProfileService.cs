namespace GuitarTrainer.Services
{
    public interface IUserProfileService
    {
        Task<bool> IsUserWithNameAsync(Guid userId);
        Task UpdateUserFullNameAsync(Guid userId, string firstName, string lastName);
    }
}
