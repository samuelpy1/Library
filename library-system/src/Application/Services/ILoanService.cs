using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace library_system.Application.Services
{
    public interface ILoanService
    {
        Task<IEnumerable<HistoryDTO>> GetAllLoansAsync();
        Task<HistoryDTO> GetLoanByIdAsync(Guid id);
        Task<HistoryDTO> CreateLoanAsync(HistoryDTO loanDto);
        Task UpdateLoanAsync(Guid id, HistoryDTO loanDto);
        Task DeleteLoanAsync(Guid id);
        Task<PagedListDto<HistoryDTO>> GetPagedLoansAsync(PaginationParams paginationParams);
    }
}
