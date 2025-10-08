using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using library_system.Domain.Entity;
using library_system.Domain.Interfaces;
using library_system.Domain.ValueObjects;

namespace library_system.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new UserDTO
            {
                UserID = u.UserID,
                Email = u.Email.Address,
                Password = u.Password.Value,
                Type = (int)u.Type
            });
        }

        public async Task<UserDTO> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return new UserDTO
            {
                UserID = user.UserID,
                Email = user.Email.Address,
                Password = user.Password.Value,
                Type = (int)user.Type
            };
        }

        public async Task<UserDTO> CreateUserAsync(UserDTO userDto)
        {
            var user = new User(Guid.NewGuid(), new Email(userDto.Email), new Password(userDto.Password), (UserType)userDto.Type);
            await _userRepository.AddAsync(user);
            return userDto;
        }

        public async Task UpdateUserAsync(Guid id, UserDTO userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found.");

            user.UpdateEmail(new Email(userDto.Email));
            user.UpdatePassword(new Password(userDto.Password));
            user.Type = (UserType)userDto.Type;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new KeyNotFoundException("User not found.");

            await _userRepository.DeleteAsync(user.UserID);
        }

        public async Task<PagedListDto<UserDTO>> GetPagedUsersAsync(PaginationParams paginationParams)
        {
            var users = _userRepository.GetAllAsQueryable(); // Assuming GetAllAsQueryable returns IQueryable<User>
            var pagedUsers = PagedListDto<User>.ToPagedList(users, paginationParams.PageNumber, paginationParams.PageSize);

            var userDtos = pagedUsers.Items.Select(u => new UserDTO
            {
                UserID = u.UserID,
                Email = u.Email.Address,
                Password = u.Password.Value,
                Type = (int)u.Type
            }).ToList();

            return new PagedListDto<UserDTO>(userDtos, pagedUsers.TotalCount, pagedUsers.CurrentPage, pagedUsers.PageSize);
        }
    }
}

