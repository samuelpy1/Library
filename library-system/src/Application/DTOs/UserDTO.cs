using System;
using library_system.Application.DTOs.HATEOAS;

namespace library_system.Application.DTOs
{
    public class MemberDTO : BaseDto
    {
        public Guid MemberId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
    }
}

