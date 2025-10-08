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
    /// Controller para gerenciamento de usuários
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// Construtor do controller de usuários
        /// </summary>
        /// <param name="userService">Serviço de usuários</param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lista todos os usuários com paginação
        /// </summary>
        /// <param name="paginationParams">Parâmetros de paginação</param>
        /// <returns>Lista paginada de usuários</returns>
        /// <response code="200">Retorna a lista de usuários com sucesso</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers([FromQuery] PaginationParams paginationParams)
        {
            var pagedUsers = await _userService.GetPagedUsersAsync(paginationParams);

            var metadata = new
            {
                pagedUsers.TotalCount,
                pagedUsers.PageSize,
                pagedUsers.CurrentPage,
                pagedUsers.TotalPages,
                pagedUsers.HasNext,
                pagedUsers.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            foreach (var user in pagedUsers.Items)
            {
                user.Links.Add(new LinkDto(Url.Link(nameof(GetUser), new { id = user.MemberId }), "self", "GET"));
                user.Links.Add(new LinkDto(Url.Link(nameof(PutUser), new { id = user.MemberId }), "update_user", "PUT"));
                user.Links.Add(new LinkDto(Url.Link(nameof(DeleteUser), new { id = user.MemberId }), "delete_user", "DELETE"));
            }

            return Ok(pagedUsers.Items);
        }

        /// <summary>
        /// Busca um usuário por ID
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Dados do usuário</returns>
        /// <response code="200">Usuário encontrado com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpGet("{id:guid}", Name = nameof(GetUser))]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetUser(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.Links.Add(new LinkDto(Url.Link(nameof(GetUser), new { id = user.MemberId }), "self", "GET"));
            user.Links.Add(new LinkDto(Url.Link(nameof(PutUser), new { id = user.MemberId }), "update_user", "PUT"));
            user.Links.Add(new LinkDto(Url.Link(nameof(DeleteUser), new { id = user.MemberId }), "delete_user", "DELETE"));

            return Ok(user);
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="userDto">Dados do usuário para atualização</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Usuário atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpPut("{id:guid}", Name = nameof(PutUser))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutUser(Guid id, UserDTO userDto)
        {
            if (id != userDto.MemberId)
            {
                return BadRequest();
            }

            try
            {
                await _userService.UpdateUserAsync(id, userDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        /// <param name="userDto">Dados do novo usuário</param>
        /// <returns>Usuário criado</returns>
        /// <response code="201">Usuário criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        [HttpPost(Name = nameof(PostUser))]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> PostUser(UserDTO userDto)
        {
            var createdUser = await _userService.CreateUserAsync(userDto);

            createdUser.Links.Add(new LinkDto(Url.Link(nameof(GetUser), new { id = createdUser.MemberId }), "self", "GET"));
            createdUser.Links.Add(new LinkDto(Url.Link(nameof(PutUser), new { id = createdUser.MemberId }), "update_user", "PUT"));
            createdUser.Links.Add(new LinkDto(Url.Link(nameof(DeleteUser), new { id = createdUser.MemberId }), "delete_user", "DELETE"));

            return CreatedAtAction(nameof(GetUser), new { id = createdUser.MemberId }, createdUser);
        }

        /// <summary>
        /// Remove um usuário
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Usuário removido com sucesso</response>
        /// <response code="404">Usuário não encontrado</response>
        [HttpDelete("{id:guid}", Name = nameof(DeleteUser))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

