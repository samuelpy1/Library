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
    /// Controller para gerenciamento de históricos de manutenção
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class HistoriesController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        /// <summary>
        /// Construtor do controller de históricos
        /// </summary>
        /// <param name="historyService">Serviço de históricos</param>
        public HistoriesController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        /// <summary>
        /// Lista todos os históricos de manutenção com paginação
        /// </summary>
        /// <param name="paginationParams">Parâmetros de paginação</param>
        /// <returns>Lista paginada de históricos de manutenção</returns>
        /// <response code="200">Retorna a lista de históricos com sucesso</response>
        [HttpGet(Name = nameof(GetMaintenanceHistories))]
        [ProducesResponseType(typeof(IEnumerable<HistoryDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<HistoryDTO>>> GetMaintenanceHistories([FromQuery] PaginationParams paginationParams)
        {
            var pagedHistories = await _historyService.GetPagedHistoriesAsync(paginationParams);

            var metadata = new
            {
                pagedHistories.TotalCount,
                pagedHistories.PageSize,
                pagedHistories.CurrentPage,
                pagedHistories.TotalPages,
                pagedHistories.HasNext,
                pagedHistories.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            foreach (var history in pagedHistories.Items)
            {
                history.Links.Add(new LinkDto(Url.Link(nameof(GetMaintenanceHistory), new { id = history.LoanId }), "self", "GET"));
                history.Links.Add(new LinkDto(Url.Link(nameof(PutMaintenanceHistory), new { id = history.LoanId }), "update_history", "PUT"));
                history.Links.Add(new LinkDto(Url.Link(nameof(DeleteMaintenanceHistory), new { id = history.LoanId }), "delete_history", "DELETE"));
            }

            return Ok(pagedHistories.Items);
        }

        /// <summary>
        /// Busca um histórico de manutenção por ID
        /// </summary>
        /// <param name="id">ID do histórico de manutenção</param>
        /// <returns>Dados do histórico de manutenção</returns>
        /// <response code="200">Histórico encontrado com sucesso</response>
        /// <response code="404">Histórico não encontrado</response>
        [HttpGet("{id:guid}", Name = nameof(GetMaintenanceHistory))]
        [ProducesResponseType(typeof(HistoryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HistoryDTO>> GetMaintenanceHistory(Guid id)
        {
            var history = await _historyService.GetHistoryByIdAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            history.Links.Add(new LinkDto(Url.Link(nameof(GetMaintenanceHistory), new { id = history.LoanId }), "self", "GET"));
            history.Links.Add(new LinkDto(Url.Link(nameof(PutMaintenanceHistory), new { id = history.LoanId }), "update_history", "PUT"));
            history.Links.Add(new LinkDto(Url.Link(nameof(DeleteMaintenanceHistory), new { id = history.LoanId }), "delete_history", "DELETE"));

            return Ok(history);
        }

        /// <summary>
        /// Atualiza um histórico de manutenção existente
        /// </summary>
        /// <param name="id">ID do histórico de manutenção</param>
        /// <param name="historyDto">Dados do histórico para atualização</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Histórico atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Histórico não encontrado</response>
        [HttpPut("{id:guid}", Name = nameof(PutMaintenanceHistory))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutMaintenanceHistory(Guid id, HistoryDTO historyDto)
        {
            if (id != historyDto.LoanId)
            {
                return BadRequest();
            }

            try
            {
                await _historyService.UpdateHistoryAsync(id, historyDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Cria um novo histórico de manutenção
        /// </summary>
        /// <param name="historyDto">Dados do novo histórico de manutenção</param>
        /// <returns>Histórico de manutenção criado</returns>
        /// <response code="201">Histórico criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        [HttpPost(Name = nameof(PostMaintenanceHistory))]
        [ProducesResponseType(typeof(HistoryDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HistoryDTO>> PostMaintenanceHistory(HistoryDTO historyDto)
        {
            var createdHistory = await _historyService.CreateHistoryAsync(historyDto);

            createdHistory.Links.Add(new LinkDto(Url.Link(nameof(GetMaintenanceHistory), new { id = createdHistory.LoanId }), "self", "GET"));
            createdHistory.Links.Add(new LinkDto(Url.Link(nameof(PutMaintenanceHistory), new { id = createdHistory.LoanId }), "update_history", "PUT"));
            createdHistory.Links.Add(new LinkDto(Url.Link(nameof(DeleteMaintenanceHistory), new { id = createdHistory.LoanId }), "delete_history", "DELETE"));

            return CreatedAtAction(nameof(GetMaintenanceHistory), new { id = createdHistory.LoanId }, createdHistory);
        }

        /// <summary>
        /// Remove um histórico de manutenção
        /// </summary>
        /// <param name="id">ID do histórico de manutenção</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Histórico removido com sucesso</response>
        /// <response code="404">Histórico não encontrado</response>
        [HttpDelete("{id:guid}", Name = nameof(DeleteMaintenanceHistory))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMaintenanceHistory(Guid id)
        {
            try
            {
                await _historyService.DeleteHistoryAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

