using System.ComponentModel.DataAnnotations;

namespace MyBuyingList.Application.Common.Options;

public class RefreshTokenOptions
{
    public const string SectionName = "RefreshTokenOptions";

    [Range(1, int.MaxValue)]
    public int ExpirationDays { get; init; } = 1;

    [Range(1, int.MaxValue)]
    public int RevokedTokenRetentionDays { get; init; } = 30;
}
