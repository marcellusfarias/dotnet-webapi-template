using MyBuyingList.Domain.Entities;

namespace MyBuyingList.Domain.Constants;

public static class Roles
{
    private const string Administrator = "Administrator";
    private const string RegularUser = "RegularUser";
    
    public static List<Role> GetValues() =>
    [
        new() { Id = 1, Name = Administrator },
        new() { Id = 2, Name = RegularUser }
    ];
}
