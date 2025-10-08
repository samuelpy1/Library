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
    public class BookService : IBookService
    {
        private readonly IRepository<Book> _bookRepository;

        public BookService(IRepository<Book> bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return books.Select(b => new BookDTO
            {
                BookId = b.BookId,
                ISBN = b.ISBN,
                Title = b.Title,
                Author = b.Author,
                Publisher = b.Publisher,
                PublicationYear = b.PublicationYear,
                Category = b.Category,
                TotalCopies = b.TotalCopies,
                AvailableCopies = b.AvailableCopies,
                Status = (int)b.Status
            });
        }

        public async Task<BookDTO> GetBookByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) return null;

            return new BookDTO
            {
                BookId = book.BookId,
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                Category = book.Category,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies,
                Status = (int)book.Status
            };
        }

        public async Task<BookDTO> CreateBookAsync(BookDTO bookDto)
        {
            var book = new Book(
                Guid.NewGuid(),
                bookDto.ISBN,
                bookDto.Title,
                bookDto.Author,
                bookDto.Publisher,
                bookDto.PublicationYear,
                bookDto.Category,
                bookDto.TotalCopies
            );
            await _bookRepository.AddAsync(book);

            return new BookDTO
            {
                BookId = book.BookId,
                ISBN = book.ISBN,
                Title = book.Title,
                Author = book.Author,
                Publisher = book.Publisher,
                PublicationYear = book.PublicationYear,
                Category = book.Category,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies,
                Status = (int)book.Status
            };
        }

        public async Task UpdateBookAsync(Guid id, BookDTO bookDto)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) throw new KeyNotFoundException("Book not found.");

            book.ISBN = bookDto.ISBN;
            book.Title = bookDto.Title;
            book.Author = bookDto.Author;
            book.Publisher = bookDto.Publisher;
            book.PublicationYear = bookDto.PublicationYear;
            book.Category = bookDto.Category;
            book.TotalCopies = bookDto.TotalCopies;
            book.AvailableCopies = bookDto.AvailableCopies;
            book.Status = (BookStatus)bookDto.Status;

            await _bookRepository.UpdateAsync(book);
        }

        public async Task DeleteBookAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            if (book == null) throw new KeyNotFoundException("Book not found.");

            await _bookRepository.DeleteAsync(book.BookId);
        }

        public async Task<PagedListDto<BookDTO>> GetPagedBooksAsync(PaginationParams paginationParams)
        {
            var books = _bookRepository.GetAllAsQueryable();
            var pagedBooks = PagedListDto<Book>.ToPagedList(books, paginationParams.PageNumber, paginationParams.PageSize);

            var bookDtos = pagedBooks.Items.Select(b => new BookDTO
            {
                BookId = b.BookId,
                ISBN = b.ISBN,
                Title = b.Title,
                Author = b.Author,
                Publisher = b.Publisher,
                PublicationYear = b.PublicationYear,
                Category = b.Category,
                TotalCopies = b.TotalCopies,
                AvailableCopies = b.AvailableCopies,
                Status = (int)b.Status
            }).ToList();

            return new PagedListDto<BookDTO>(bookDtos, pagedBooks.TotalCount, pagedBooks.CurrentPage, pagedBooks.PageSize);
        }
    }
}
