namespace MyBuyingList.Application.Common.Options;

public class RefreshTokenOptions
{
    public const string SectionName = "RefreshTokenOptions";
    public int ExpirationDays { get; init; } = 1;
    public int RevokedTokenRetentionDays { get; init; } = 30;
}
