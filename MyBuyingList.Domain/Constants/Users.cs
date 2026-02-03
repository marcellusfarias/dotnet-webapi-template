using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Domain.Constants;

public static class Users
{
    public static readonly User AdminUser = new()
    { 
        Id = 1,
        Email = "marcelluscfarias@gmail.com",
        UserName = "admin",
        Password = "$2a$16$CZ18qbFWtcoAY6SnsqNYnO1H.D3It5TTD6uuhTFyjge5I/n5SRLKe",
        Active = true 
    };

    public static IEnumerable<User> GetValues()
    {
        List<User> users = [AdminUser];
        
        return users;
    }
}
