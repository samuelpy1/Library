using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace library_system.Application.Services
{
    public interface ILoanService
    {
        Task<IEnumerable<LoanDTO>> GetAllLoansAsync();
        Task<LoanDTO> GetLoanByIdAsync(Guid id);
        Task<LoanDTO> CreateLoanAsync(LoanDTO loanDto);
        Task UpdateLoanAsync(Guid id, LoanDTO loanDto);
        Task DeleteLoanAsync(Guid id);
        Task<PagedListDto<LoanDTO>> GetPagedLoansAsync(PaginationParams paginationParams);
    }
}
