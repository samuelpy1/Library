using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using library_system.Application.DTOs;
using library_system.Application.Services;
using library_system.Application.DTOs.Pagination;
using library_system.Application.DTOs.HATEOAS;
using System.Text.Json;

namespace library_system.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de livros
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        /// <summary>
        /// Construtor do controller de livros
        /// </summary>
        /// <param name="bookService">Serviço de livros</param>
        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Lista todos os livros com paginação
        /// </summary>
        /// <param name="paginationParams">Parâmetros de paginação</param>
        /// <returns>Lista paginada de livros</returns>
        /// <response code="200">Retorna a lista de livros com sucesso</response>
        [HttpGet(Name = nameof(GetBooks))]
        [ProducesResponseType(typeof(IEnumerable<BookDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooks([FromQuery] PaginationParams paginationParams)
        {
            var pagedBooks = await _bookService.GetPagedBooksAsync(paginationParams);

            var metadata = new
            {
                pagedBooks.TotalCount,
                pagedBooks.PageSize,
                pagedBooks.CurrentPage,
                pagedBooks.TotalPages,
                pagedBooks.HasNext,
                pagedBooks.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            foreach (var book in pagedBooks.Items)
            {
                book.Links.Add(new LinkDto(Url.Link(nameof(GetBook), new { id = book.BookId }), "self", "GET"));
                book.Links.Add(new LinkDto(Url.Link(nameof(PutBook), new { id = book.BookId }), "update_book", "PUT"));
                book.Links.Add(new LinkDto(Url.Link(nameof(DeleteBook), new { id = book.BookId }), "delete_book", "DELETE"));
            }

            return Ok(pagedBooks.Items);
        }

        /// <summary>
        /// Busca um livro por ID
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <returns>Dados do livro</returns>
        /// <response code="200">Livro encontrado com sucesso</response>
        /// <response code="404">Livro não encontrado</response>
        [HttpGet("{id:guid}", Name = nameof(GetBook))]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDTO>> GetBook(Guid id)
        {
            var book = await _bookService.GetBookByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            book.Links.Add(new LinkDto(Url.Link(nameof(GetBook), new { id = book.BookId }), "self", "GET"));
            book.Links.Add(new LinkDto(Url.Link(nameof(PutBook), new { id = book.BookId }), "update_book", "PUT"));
            book.Links.Add(new LinkDto(Url.Link(nameof(DeleteBook), new { id = book.BookId }), "delete_book", "DELETE"));

            return Ok(book);
        }

        /// <summary>
        /// Atualiza um livro existente
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <param name="bookDto">Dados do livro para atualização</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Livro atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Livro não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:guid}", Name = nameof(PutBook))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutBook(Guid id, [FromBody] BookDTO bookDto)
        {
            if (id != bookDto.BookId)
            {
                return BadRequest(new { error = "O ID na URL não corresponde ao ID do livro no corpo da requisição." });
            }

            try
            {
                await _bookService.UpdateBookAsync(id, bookDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Livro com ID {id} não encontrado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar livro: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                return StatusCode(500, new { error = "Erro interno", message = ex.Message, innerException = ex.InnerException?.Message });
            }

            return NoContent();
        }

        /// <summary>
        /// Cria um novo livro
        /// </summary>
        /// <param name="bookDto">Dados do novo livro</param>
        /// <returns>Livro criado</returns>
        /// <response code="201">Livro criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        [HttpPost(Name = nameof(PostBook))]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookDTO>> PostBook(BookDTO bookDto)
        {
            try
            {
                var createdBook = await _bookService.CreateBookAsync(bookDto);

                createdBook.Links.Add(new LinkDto(Url.Link(nameof(GetBook), new { id = createdBook.BookId }), "self", "GET"));
                createdBook.Links.Add(new LinkDto(Url.Link(nameof(PutBook), new { id = createdBook.BookId }), "update_book", "PUT"));
                createdBook.Links.Add(new LinkDto(Url.Link(nameof(DeleteBook), new { id = createdBook.BookId }), "delete_book", "DELETE"));

                return CreatedAtAction(nameof(GetBook), new { id = createdBook.BookId }, createdBook);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Remove um livro
        /// </summary>
        /// <param name="id">ID do livro</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Livro removido com sucesso</response>
        /// <response code="404">Livro não encontrado</response>
        [HttpDelete("{id:guid}", Name = nameof(DeleteBook))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            try
            {
                await _bookService.DeleteBookAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
