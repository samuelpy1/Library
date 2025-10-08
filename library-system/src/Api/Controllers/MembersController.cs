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
    /// Controller para gerenciamento de membros
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;

        /// <summary>
        /// Construtor do controller de membros
        /// </summary>
        /// <param name="memberService">Serviço de membros</param>
        public MembersController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        /// <summary>
        /// Lista todos os membros com paginação
        /// </summary>
        /// <param name="paginationParams">Parâmetros de paginação</param>
        /// <returns>Lista paginada de membros</returns>
        /// <response code="200">Retorna a lista de membros com sucesso</response>
        [HttpGet(Name = nameof(GetMembers))]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetMembers([FromQuery] PaginationParams paginationParams)
        {
            var pagedMembers = await _memberService.GetPagedMembersAsync(paginationParams);

            var metadata = new
            {
                pagedMembers.TotalCount,
                pagedMembers.PageSize,
                pagedMembers.CurrentPage,
                pagedMembers.TotalPages,
                pagedMembers.HasNext,
                pagedMembers.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(metadata));

            foreach (var member in pagedMembers.Items)
            {
                member.Links.Add(new LinkDto(Url.Link(nameof(GetMember), new { id = member.MemberId }), "self", "GET"));
                member.Links.Add(new LinkDto(Url.Link(nameof(PutMember), new { id = member.MemberId }), "update_member", "PUT"));
                member.Links.Add(new LinkDto(Url.Link(nameof(DeleteMember), new { id = member.MemberId }), "delete_member", "DELETE"));
            }

            return Ok(pagedMembers.Items);
        }

        /// <summary>
        /// Busca um membro por ID
        /// </summary>
        /// <param name="id">ID do membro</param>
        /// <returns>Dados do membro</returns>
        /// <response code="200">Membro encontrado com sucesso</response>
        /// <response code="404">Membro não encontrado</response>
        [HttpGet("{id:guid}", Name = nameof(GetMember))]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDTO>> GetMember(Guid id)
        {
            var member = await _memberService.GetMemberByIdAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            member.Links.Add(new LinkDto(Url.Link(nameof(GetMember), new { id = member.MemberId }), "self", "GET"));
            member.Links.Add(new LinkDto(Url.Link(nameof(PutMember), new { id = member.MemberId }), "update_member", "PUT"));
            member.Links.Add(new LinkDto(Url.Link(nameof(DeleteMember), new { id = member.MemberId }), "delete_member", "DELETE"));

            return Ok(member);
        }

        /// <summary>
        /// Atualiza um membro existente
        /// </summary>
        /// <param name="id">ID do membro</param>
        /// <param name="memberDto">Dados do membro para atualização</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Membro atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Membro não encontrado</response>
        [HttpPut("{id:guid}", Name = nameof(PutMember))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PutMember(Guid id, UserDTO memberDto)
        {
            if (id != memberDto.MemberId)
            {
                return BadRequest();
            }

            try
            {
                await _memberService.UpdateMemberAsync(id, memberDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Cria um novo membro
        /// </summary>
        /// <param name="memberDto">Dados do novo membro</param>
        /// <returns>Membro criado</returns>
        /// <response code="201">Membro criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        [HttpPost(Name = nameof(PostMember))]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDTO>> PostMember(UserDTO memberDto)
        {
            var createdMember = await _memberService.CreateMemberAsync(memberDto);

            createdMember.Links.Add(new LinkDto(Url.Link(nameof(GetMember), new { id = createdMember.MemberId }), "self", "GET"));
            createdMember.Links.Add(new LinkDto(Url.Link(nameof(PutMember), new { id = createdMember.MemberId }), "update_member", "PUT"));
            createdMember.Links.Add(new LinkDto(Url.Link(nameof(DeleteMember), new { id = createdMember.MemberId }), "delete_member", "DELETE"));

            return CreatedAtAction(nameof(GetMember), new { id = createdMember.MemberId }, createdMember);
        }

        /// <summary>
        /// Remove um membro
        /// </summary>
        /// <param name="id">ID do membro</param>
        /// <returns>Resultado da operação</returns>
        /// <response code="204">Membro removido com sucesso</response>
        /// <response code="404">Membro não encontrado</response>
        [HttpDelete("{id:guid}", Name = nameof(DeleteMember))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMember(Guid id)
        {
            try
            {
                await _memberService.DeleteMemberAsync(id);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
