namespace GuitarTrainer.Services
{
    public static class UserIdParsingService
    {
        public static Guid ParseUserId(string userId)
        {
            if (!Guid.TryParse(userId, out Guid userGuidId))
            {
                //??? НАСТРОИТЬ УЖЕ ИСКЛЮЧЕНИЯ!!!
            }
            return userGuidId;
        }
    }
}
