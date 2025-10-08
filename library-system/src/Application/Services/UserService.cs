using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using library_system.Domain.Entity;
using library_system.Domain.Interfaces;
using library_system.Domain.ValueObjects;

namespace library_system.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<Member> _userRepository;

        public UserService(IRepository<Member> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDTO
            {
                MemberId = u.MemberId,
                Name = u.Name,
                Email = u.Email.Address,
                Password = u.Password.Value,
                Phone = u.Phone,
                RegistrationDate = u.RegistrationDate,
                IsActive = u.IsActive
            });
        }

        public async Task<UserDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserDTO
            {
                MemberId = user.MemberId,
                Name = user.Name,
                Email = user.Email.Address,
                Password = user.Password.Value,
                Phone = user.Phone,
                RegistrationDate = user.RegistrationDate,
                IsActive = user.IsActive
            };
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO userDto)
        {
            var user = new Member(
                Guid.NewGuid(),
                userDto.Name,
                new Email(userDto.Email),
                new Password(userDto.Password),
                userDto.Phone
            );
            await _userRepository.AddAsync(user);
            return userDto;
        }

        public async Task UpdateUserAsync(Guid id, UserDTO userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found.");

            user.Name = userDto.Name;
            user.UpdateEmail(new Email(userDto.Email));
            user.UpdatePassword(new Password(userDto.Password));
            user.Phone = userDto.Phone;
            user.IsActive = userDto.IsActive;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found.");

            await _userRepository.DeleteAsync(user.MemberId);
        }

        public async Task<PagedListDto<UserDTO>> GetPagedUsersAsync(PaginationParams paginationParams)
        {
            var users = _userRepository.GetAllAsQueryable();
            var pagedUsers = PagedListDto<Member>.ToPagedList(users, paginationParams.PageNumber, paginationParams.PageSize);

            var userDtos = pagedUsers.Items.Select(u => new UserDTO
            {
                MemberId = u.MemberId,
                Name = u.Name,
                Email = u.Email.Address,
                Password = u.Password.Value,
                Phone = u.Phone,
                RegistrationDate = u.RegistrationDate,
                IsActive = u.IsActive
            }).ToList();

            return new PagedListDto<UserDTO>(userDtos, pagedUsers.TotalCount, pagedUsers.CurrentPage, pagedUsers.PageSize);
        }
    }
}

