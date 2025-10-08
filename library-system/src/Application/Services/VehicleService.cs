using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using library_system.Domain.Entity;
using library_system.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace library_system.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IRepository<Book> _vehicleRepository;

        public VehicleService(IRepository<Book> vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<IEnumerable<BookDTO>> GetAllVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAllAsync();
            return vehicles.Select(v => new BookDTO
            {
                BookId = v.BookId,
                ISBN = v.ISBN,
                Title = v.Title,
                Author = v.Author,
                Publisher = v.Publisher,
                PublicationYear = v.PublicationYear,
                Category = v.Category,
                TotalCopies = v.TotalCopies,
                AvailableCopies = v.AvailableCopies,
                Status = (int)v.Status
            });
        }

        public async Task<BookDTO> GetVehicleByIdAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) return null;

            return new BookDTO
            {
                BookId = vehicle.BookId,
                ISBN = vehicle.ISBN,
                Title = vehicle.Title,
                Author = vehicle.Author,
                Publisher = vehicle.Publisher,
                PublicationYear = vehicle.PublicationYear,
                Category = vehicle.Category,
                TotalCopies = vehicle.TotalCopies,
                AvailableCopies = vehicle.AvailableCopies,
                Status = (int)vehicle.Status
            };
        }

        public async Task<BookDTO> CreateVehicleAsync(BookDTO vehicleDto)
        {
            var vehicle = new Book(
                Guid.NewGuid(),
                vehicleDto.ISBN,
                vehicleDto.Title,
                vehicleDto.Author,
                vehicleDto.Publisher,
                vehicleDto.PublicationYear,
                vehicleDto.Category,
                vehicleDto.TotalCopies
            );
            await _vehicleRepository.AddAsync(vehicle);
            return new BookDTO
            {
                BookId = vehicle.BookId,
                ISBN = vehicle.ISBN,
                Title = vehicle.Title,
                Author = vehicle.Author,
                Publisher = vehicle.Publisher,
                PublicationYear = vehicle.PublicationYear,
                Category = vehicle.Category,
                TotalCopies = vehicle.TotalCopies,
                AvailableCopies = vehicle.AvailableCopies,
                Status = (int)vehicle.Status
            };
        }

        public async Task UpdateVehicleAsync(Guid id, BookDTO vehicleDto)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) throw new KeyNotFoundException("Vehicle not found.");

            vehicle.ISBN = vehicleDto.ISBN;
            vehicle.Title = vehicleDto.Title;
            vehicle.Author = vehicleDto.Author;
            vehicle.Publisher = vehicleDto.Publisher;
            vehicle.PublicationYear = vehicleDto.PublicationYear;
            vehicle.Category = vehicleDto.Category;
            vehicle.TotalCopies = vehicleDto.TotalCopies;
            vehicle.AvailableCopies = vehicleDto.AvailableCopies;
            vehicle.Status = (BookStatus)vehicleDto.Status;

            await _vehicleRepository.UpdateAsync(vehicle);
        }

        public async Task DeleteVehicleAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id);
            if (vehicle == null) throw new KeyNotFoundException("Vehicle not found.");

            await _vehicleRepository.DeleteAsync(vehicle.BookId);
        }

        public async Task<PagedListDto<BookDTO>> GetPagedVehiclesAsync(PaginationParams paginationParams)
        {
            var vehicles = _vehicleRepository.GetAllAsQueryable();
            var pagedVehicles = PagedListDto<Book>.ToPagedList(vehicles, paginationParams.PageNumber, paginationParams.PageSize);

            var vehicleDtos = pagedVehicles.Items.Select(v => new BookDTO
            {
                BookId = v.BookId,
                ISBN = v.ISBN,
                Title = v.Title,
                Author = v.Author,
                Publisher = v.Publisher,
                PublicationYear = v.PublicationYear,
                Category = v.Category,
                TotalCopies = v.TotalCopies,
                AvailableCopies = v.AvailableCopies,
                Status = (int)v.Status
            }).ToList();

            return new PagedListDto<BookDTO>(vehicleDtos, pagedVehicles.TotalCount, pagedVehicles.CurrentPage, pagedVehicles.PageSize);
        }
    }
}

