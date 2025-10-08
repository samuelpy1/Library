using Microsoft.AspNetCore.Http;
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
    /// Controller para gerenciamento de empréstimos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;

        /// <summary>
        /// Construtor do controller de empréstimos
        /// </summary>
        /// <param name="loanService">Serviço de empréstimos</param>
        public LoansController(ILoanService loanService)
        {
            _loanService = loanService;
        }

        /// <summary>
        /// Lista todos os empréstimos com paginação
        /// </summary>
        /// <param name="paginationParams">Parâmetros de paginação</param>
        /// <returns>Lista paginada de empréstimos</returns>
        /// <response code="200">Retorna a lista de empréstimos com sucesso</response>
        [HttpGet(Name = nameof(GetLoans))]
        [ProducesResponseType(typeof(IEnumerable<LoanDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LoanDTO>>> GetLoans([FromQuery] PaginationParams paginationParams)
        {
            var pagedLoans = await _loanService.GetPagedLoansAsync(paginationParams);

            var metadata = new
            {
                pagedLoans.TotalCount,
                pagedLoans.PageSize,
                pagedLoans.CurrentPage,
                pagedLoans.TotalPages,
                pagedLoans.HasNext,
                pagedLoans.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            foreach (var loan in pagedLoans.Items)
            {
                loan.Links.Add(new LinkDto(Url.Link(nameof(GetLoan), new { id = loan.LoanId }), "self", "GET"));
                loan.Links.Add(new LinkDto(Url.Link(nameof(PutLoan), new { id = loan.LoanId }), "update_loan", "PUT"));
                loan.Links.Add(new LinkDto(Url.Link(nameof(DeleteLoan), new { id = loan.LoanId }), "delete_loan", "DELETE"));
            }

            return Ok(pagedLoans.Items);
        }

        /// <summary>
        /// Busca um empréstimo por ID
        /// </summary>
        /// <param name="id">ID do empréstimo</param>
        /// <returns>Dados do empréstimo</returns>
        /// <response code="200">Empréstimo encontrado com sucesso</response>
        /// <response code="404">Empréstimo não encontrado</response>
        [HttpGet("{id:guid}", Name = nameof(GetLoan))]
        [ProducesResponseType(typeof(LoanDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LoanDTO>> GetLoan(Guid id)
        {
            var loan = await _loanService.GetLoanByIdAsync(id);
            if (loan == null)
            {
                return NotFound();
            }

            loan.Links.Add(new LinkDto(Url.Link(nameof(GetLoan), new { id = loan.LoanId }), "self", "GET"));
            loan.Links.Add(new LinkDto(Url.Link(nameof(PutLoan), new { id = loan.LoanId }), "update_loan", "PUT"));
            loan.Links.Add(new LinkDto(Url.Link(nameof(DeleteLoan), new { id = loan.LoanId }), "delete_loan", "DELETE"));

            return Ok(loan);
        }

        /// <summary>
        /// Atualiza um empréstimo existente
        /// </summary>
        /// <param name="id">ID do empréstimo</param>
        /// <param name="loanDto">Dados do empréstimo para atualização</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Empréstimo atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Empréstimo não encontrado</response>
        [HttpPut("{id:guid}", Name = nameof(PutLoan))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutLoan(Guid id, LoanDTO loanDto)
        {
            if (id != loanDto.LoanId)
            {
                return BadRequest();
            }

            try
            {
                await _loanService.UpdateLoanAsync(id, loanDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Cria um novo empréstimo
        /// </summary>
        /// <param name="loanDto">Dados do novo empréstimo</param>
        /// <returns>Empréstimo criado</returns>
        /// <response code="201">Empréstimo criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        [HttpPost(Name = nameof(PostLoan))]
        [ProducesResponseType(typeof(LoanDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoanDTO>> PostLoan(LoanDTO loanDto)
        {
            var createdLoan = await _loanService.CreateLoanAsync(loanDto);

            createdLoan.Links.Add(new LinkDto(Url.Link(nameof(GetLoan), new { id = createdLoan.LoanId }), "self", "GET"));
            createdLoan.Links.Add(new LinkDto(Url.Link(nameof(PutLoan), new { id = createdLoan.LoanId }), "update_loan", "PUT"));
            createdLoan.Links.Add(new LinkDto(Url.Link(nameof(DeleteLoan), new { id = createdLoan.LoanId }), "delete_loan", "DELETE"));

            return CreatedAtAction(nameof(GetLoan), new { id = createdLoan.LoanId }, createdLoan);
        }

        /// <summary>
        /// Remove um empréstimo
        /// </summary>
        /// <param name="id">ID do empréstimo</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Empréstimo removido com sucesso</response>
        /// <response code="404">Empréstimo não encontrado</response>
        [HttpDelete("{id:guid}", Name = nameof(DeleteLoan))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLoan(Guid id)
        {
            try
            {
                await _loanService.DeleteLoanAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
