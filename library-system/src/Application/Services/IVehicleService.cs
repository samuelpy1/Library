using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace library_system.Application.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDTO>> GetAllVehiclesAsync();
        Task<VehicleDTO> GetVehicleByIdAsync(Guid id);
        Task<VehicleDTO> CreateVehicleAsync(VehicleDTO vehicleDto);
        Task UpdateVehicleAsync(Guid id, VehicleDTO vehicleDto);
        Task DeleteVehicleAsync(Guid id);
        Task<PagedListDto<VehicleDTO>> GetPagedVehiclesAsync(PaginationParams paginationParams);
    }
}

