namespace MyBuyingList.Application.Common.Options;

public class LockoutOptions
{
    public const string SectionName = "LockoutOptions";
    public int MaxFailedAttempts { get; init; } = 5;
    public int LockoutDurationMinutes { get; init; } = 15;
}
