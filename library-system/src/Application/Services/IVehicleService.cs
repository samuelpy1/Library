using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace library_system.Application.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<BookDTO>> GetAllVehiclesAsync();
        Task<BookDTO> GetVehicleByIdAsync(Guid id);
        Task<BookDTO> CreateVehicleAsync(BookDTO vehicleDto);
        Task UpdateVehicleAsync(Guid id, BookDTO vehicleDto);
        Task DeleteVehicleAsync(Guid id);
        Task<PagedListDto<BookDTO>> GetPagedVehiclesAsync(PaginationParams paginationParams);
    }
}

