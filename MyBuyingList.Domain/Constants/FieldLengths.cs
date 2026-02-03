// ReSharper disable InconsistentNaming
namespace MyBuyingList.Domain.Constants;

// follow pattern: [ENTITY]_[PROPERTY]_[CONSTRAINT]
public static class FieldLengths
{
    // Policy
    public const int POLICY_NAME_MAX_LENGTH = 32;

    // Role
    public const int ROLE_NAME_MAX_LENGTH = 32;

    // User
    public const int USER_USERNAME_MIN_LENGTH = 3;
    public const int USER_USERNAME_MAX_LENGTH = 32;
    public const int USER_EMAIL_MAX_LENGTH = 254;
    public const int USER_PASSWORD_MAX_LENGTH = 72;
}
