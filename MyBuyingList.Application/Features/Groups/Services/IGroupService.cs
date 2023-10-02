using MyBuyingList.Application.Features.Groups.DTOs;

namespace MyBuyingList.Application.Features.Groups.Services;

public interface IGroupService
{
    /// <summary>
    /// Return group by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// /// <exception cref="">Throws ResourceNotFound</exception>
    Task<GetGroupDto?> GetByIdAsync(int id, CancellationToken token);
    /// <summary>
    /// Creates new group.
    /// </summary>
    /// <param name="groupDto"></param>
    /// <param name="currentUserId"></param>
    /// <returns>The new group Id.</returns>
    Task<int> CreateAsync(CreateGroupDto groupDto, int currentUserId, CancellationToken token);
    Task ChangeNameAsync(UpdateGroupNameDto dto, CancellationToken token);
    /// <summary>
    /// Delete group if no Buying List associated with it.
    /// </summary>
    /// <param name="groupId"></param>
    /// <exception cref="">Throws ResourceNotFound</exception>
    Task DeleteAsync(int groupId, CancellationToken token);
}
