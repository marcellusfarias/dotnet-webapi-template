using MyBuyingList.Application.Contracts.GroupDtos;

namespace MyBuyingList.Application.Common.Interfaces.Services;

public interface IGroupService
{
    /// <summary>
    /// Return group by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// /// <exception cref="">Throws ResourceNotFound</exception>
    Task<GetGroupDto?> GetByIdAsync(int id);
    /// <summary>
    /// Creates new group.
    /// </summary>
    /// <param name="groupDto"></param>
    /// <param name="currentUserId"></param>
    /// <returns>The new group Id.</returns>
    Task<int> CreateAsync(CreateGroupDto groupDto, int currentUserId);
    Task ChangeNameAsync(UpdateGroupNameDto dto);
    /// <summary>
    /// Delete group if no Buying List associated with it.
    /// </summary>
    /// <param name="groupId"></param>
    /// <exception cref="">Throws ResourceNotFound</exception>
    Task DeleteAsync(int groupId);
}
