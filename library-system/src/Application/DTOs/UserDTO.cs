using System;
using library_system.Application.DTOs.HATEOAS;

namespace library_system.Application.DTOs
{
    public class UserDTO : BaseDto
    {
        public Guid MemberId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
