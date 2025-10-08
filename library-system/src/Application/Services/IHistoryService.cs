using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace library_system.Application.Services
{
    public interface IHistoryService
    {
        Task<IEnumerable<HistoryDTO>> GetAllHistoriesAsync();
        Task<HistoryDTO> GetHistoryByIdAsync(Guid id);
        Task<HistoryDTO> CreateHistoryAsync(HistoryDTO historyDto);
        Task UpdateHistoryAsync(Guid id, HistoryDTO historyDto);
        Task DeleteHistoryAsync(Guid id);
        Task<PagedListDto<HistoryDTO>> GetPagedHistoriesAsync(PaginationParams paginationParams);
    }
}

