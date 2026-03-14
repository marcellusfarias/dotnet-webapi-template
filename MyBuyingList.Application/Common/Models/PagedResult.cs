namespace MyBuyingList.Application.Common.Models;

public record PagedResult<T>(IEnumerable<T> Data, int Page, int TotalCount, int TotalPages);
