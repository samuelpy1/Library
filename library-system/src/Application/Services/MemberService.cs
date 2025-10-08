using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using library_system.Domain.Entity;
using library_system.Domain.Interfaces;
using library_system.Domain.ValueObjects;

namespace library_system.Application.Services
{
    public class MemberService : IMemberService
    {
        private readonly IRepository<Member> _memberRepository;

        public MemberService(IRepository<Member> memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<IEnumerable<MemberDTO>> GetAllMembersAsync()
        {
            var members = await _memberRepository.GetAllAsync();
            return members.Select(m => new MemberDTO
            {
                MemberId = m.MemberId,
                Name = m.Name,
                Email = m.Email.Address,
                Password = m.Password.Value,
                Phone = m.Phone,
                RegistrationDate = m.RegistrationDate,
                IsActive = m.IsActive
            });
        }

        public async Task<MemberDTO> GetMemberByIdAsync(Guid id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null) return null;

            return new MemberDTO
            {
                MemberId = member.MemberId,
                Name = member.Name,
                Email = member.Email.Address,
                Password = member.Password.Value,
                Phone = member.Phone,
                RegistrationDate = member.RegistrationDate,
                IsActive = member.IsActive
            };
        }

        public async Task<MemberDTO> CreateMemberAsync(MemberDTO memberDto)
        {
            var member = new Member(
                Guid.NewGuid(),
                memberDto.Name,
                new Email(memberDto.Email),
                new Password(memberDto.Password),
                memberDto.Phone
            );
            await _memberRepository.AddAsync(member);

            return new MemberDTO
            {
                MemberId = member.MemberId,
                Name = member.Name,
                Email = member.Email.Address,
                Password = member.Password.Value,
                Phone = member.Phone,
                RegistrationDate = member.RegistrationDate,
                IsActive = member.IsActive
            };
        }

        public async Task UpdateMemberAsync(Guid id, MemberDTO memberDto)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null) throw new KeyNotFoundException("Member not found.");

            member.Name = memberDto.Name;
            member.UpdateEmail(new Email(memberDto.Email));
            member.UpdatePassword(new Password(memberDto.Password));
            member.Phone = memberDto.Phone;
            member.IsActive = memberDto.IsActive;

            await _memberRepository.UpdateAsync(member);
        }

        public async Task DeleteMemberAsync(Guid id)
        {
            var member = await _memberRepository.GetByIdAsync(id);
            if (member == null) throw new KeyNotFoundException("Member not found.");

            await _memberRepository.DeleteAsync(member.MemberId);
        }

        public async Task<PagedListDto<MemberDTO>> GetPagedMembersAsync(PaginationParams paginationParams)
        {
            var members = _memberRepository.GetAllAsQueryable();
            var pagedMembers = PagedListDto<Member>.ToPagedList(members, paginationParams.PageNumber, paginationParams.PageSize);

            var memberDtos = pagedMembers.Items.Select(m => new MemberDTO
            {
                MemberId = m.MemberId,
                Name = m.Name,
                Email = m.Email.Address,
                Password = m.Password.Value,
                Phone = m.Phone,
                RegistrationDate = m.RegistrationDate,
                IsActive = m.IsActive
            }).ToList();

            return new PagedListDto<MemberDTO>(memberDtos, pagedMembers.TotalCount, pagedMembers.CurrentPage, pagedMembers.PageSize);
        }
    }
}
