using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace library_system.Application.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDTO>> GetAllBooksAsync();
        Task<BookDTO> GetBookByIdAsync(Guid id);
        Task<BookDTO> CreateBookAsync(BookDTO bookDto);
        Task UpdateBookAsync(Guid id, BookDTO bookDto);
        Task DeleteBookAsync(Guid id);
        Task<PagedListDto<BookDTO>> GetPagedBooksAsync(PaginationParams paginationParams);
    }
}
