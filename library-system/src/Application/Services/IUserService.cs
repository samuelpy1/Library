using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using library_system.Domain.Entity;

namespace library_system.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(Guid id);
        Task<UserDTO> CreateUserAsync(UserDTO userDto);
        Task UpdateUserAsync(Guid id, UserDTO userDto);
        Task DeleteUserAsync(Guid id);
        Task<PagedListDto<UserDTO>> GetPagedUsersAsync(PaginationParams paginationParams);
    }
}

