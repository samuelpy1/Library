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
    /// Controller para gerenciamento de motocicletas
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _vehicleService;

        /// <summary>
        /// Construtor do controller de veículos
        /// </summary>
        /// <param name="vehicleService">Serviço de veículos</param>
        public VehiclesController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        /// <summary>
        /// Lista todas as motocicletas com paginação
        /// </summary>
        /// <param name="paginationParams">Parâmetros de paginação</param>
        /// <returns>Lista paginada de motocicletas</returns>
        /// <response code="200">Retorna a lista de motocicletas com sucesso</response>
        [HttpGet(Name = nameof(GetVehicles))]
        [ProducesResponseType(typeof(IEnumerable<BookDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetVehicles([FromQuery] PaginationParams paginationParams)
        {
            var pagedVehicles = await _vehicleService.GetPagedVehiclesAsync(paginationParams);

            var metadata = new
            {
                pagedVehicles.TotalCount,
                pagedVehicles.PageSize,
                pagedVehicles.CurrentPage,
                pagedVehicles.TotalPages,
                pagedVehicles.HasNext,
                pagedVehicles.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            foreach (var vehicle in pagedVehicles.Items)
            {
                vehicle.Links.Add(new LinkDto(Url.Link(nameof(GetVehicle), new { id = vehicle.BookId }), "self", "GET"));
                vehicle.Links.Add(new LinkDto(Url.Link(nameof(PutVehicle), new { id = vehicle.BookId }), "update_vehicle", "PUT"));
                vehicle.Links.Add(new LinkDto(Url.Link(nameof(DeleteVehicle), new { id = vehicle.BookId }), "delete_vehicle", "DELETE"));
            }

            return Ok(pagedVehicles.Items);
        }

        /// <summary>
        /// Busca uma motocicleta por ID
        /// </summary>
        /// <param name="id">ID da motocicleta</param>
        /// <returns>Dados da motocicleta</returns>
        /// <response code="200">Motocicleta encontrada com sucesso</response>
        /// <response code="404">Motocicleta não encontrada</response>
        [HttpGet("{id:guid}", Name = nameof(GetVehicle))]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookDTO>> GetVehicle(Guid id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            vehicle.Links.Add(new LinkDto(Url.Link(nameof(GetVehicle), new { id = vehicle.BookId }), "self", "GET"));
            vehicle.Links.Add(new LinkDto(Url.Link(nameof(PutVehicle), new { id = vehicle.BookId }), "update_vehicle", "PUT"));
            vehicle.Links.Add(new LinkDto(Url.Link(nameof(DeleteVehicle), new { id = vehicle.BookId }), "delete_vehicle", "DELETE"));

            return Ok(vehicle);
        }

        /// <summary>
        /// Atualiza uma motocicleta existente
        /// </summary>
        /// <param name="id">ID da motocicleta</param>
        /// <param name="vehicleDto">Dados da motocicleta para atualização</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Motocicleta atualizada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Motocicleta não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:guid}", Name = nameof(PutVehicle))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutVehicle(Guid id, [FromBody] BookDTO vehicleDto)
        {
            if (id != vehicleDto.BookId)
            {
                return BadRequest(new { error = "O ID na URL não corresponde ao ID do veículo no corpo da requisição." });
            }

            try
            {
                await _vehicleService.UpdateVehicleAsync(id, vehicleDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Veículo com ID {id} não encontrado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar veículo: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                return StatusCode(500, new { error = "Erro interno", message = ex.Message, innerException = ex.InnerException?.Message });
            }

            return NoContent();
        }

        /// <summary>
        /// Cria uma nova motocicleta
        /// </summary>
        /// <param name="vehicleDto">Dados da nova motocicleta</param>
        /// <returns>Motocicleta criada</returns>
        /// <response code="201">Motocicleta criada com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        [HttpPost(Name = nameof(PostVehicle))]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookDTO>> PostVehicle(BookDTO vehicleDto)
        {
            try
            {
                var createdVehicle = await _vehicleService.CreateVehicleAsync(vehicleDto);

                createdVehicle.Links.Add(new LinkDto(Url.Link(nameof(GetVehicle), new { id = createdVehicle.BookId }), "self", "GET"));
                createdVehicle.Links.Add(new LinkDto(Url.Link(nameof(PutVehicle), new { id = createdVehicle.BookId }), "update_vehicle", "PUT"));
                createdVehicle.Links.Add(new LinkDto(Url.Link(nameof(DeleteVehicle), new { id = createdVehicle.BookId }), "delete_vehicle", "DELETE"));

                return CreatedAtAction(nameof(GetVehicle), new { id = createdVehicle.BookId }, createdVehicle);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Remove uma motocicleta
        /// </summary>
        /// <param name="id">ID da motocicleta</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Motocicleta removida com sucesso</response>
        /// <response code="404">Motocicleta não encontrada</response>
        [HttpDelete("{id:guid}", Name = nameof(DeleteVehicle))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVehicle(Guid id)
        {
            try
            {
                await _vehicleService.DeleteVehicleAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

