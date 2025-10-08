using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using library_system.Domain.Entity;

namespace library_system.Application.Services
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberDTO>> GetAllMembersAsync();
        Task<MemberDTO> GetMemberByIdAsync(Guid id);
        Task<MemberDTO> CreateMemberAsync(MemberDTO memberDto);
        Task UpdateMemberAsync(Guid id, MemberDTO memberDto);
        Task DeleteMemberAsync(Guid id);
        Task<PagedListDto<MemberDTO>> GetPagedMembersAsync(PaginationParams paginationParams);
    }
}
