using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using library_system.Domain.Entity;

namespace library_system.Application.Services
{
    public interface IMemberService
    {
        Task<IEnumerable<UserDTO>> GetAllMembersAsync();
        Task<UserDTO> GetMemberByIdAsync(Guid id);
        Task<UserDTO> CreateMemberAsync(UserDTO memberDto);
        Task UpdateMemberAsync(Guid id, UserDTO memberDto);
        Task DeleteMemberAsync(Guid id);
        Task<PagedListDto<UserDTO>> GetPagedMembersAsync(PaginationParams paginationParams);
    }
}
