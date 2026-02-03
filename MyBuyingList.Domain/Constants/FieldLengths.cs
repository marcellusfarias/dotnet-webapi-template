namespace MyBuyingList.Domain.Constants;

// follow pattern: [ENTITY]_[PROPERTY]_{MESSAGE}
public static class FieldLengths
{
    // Policy
    public static readonly int POLICY_NAME_MAX_LENGTH = 32;

    // Role
    public static readonly int ROLE_NAME_MAX_LENGTH = 32;

    // User
    public static readonly int USER_USERNAME_MIN_LENGTH = 3;
    public static readonly int USER_USERNAME_MAX_LENGTH = 32;
    public static readonly int USER_EMAIL_MAX_LENGTH = 254;
    public static readonly int USER_PASSWORD_MAX_LENGTH = 72;
}
