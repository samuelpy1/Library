using library_system.Application.DTOs.HATEOAS;
using System.Collections.Generic;

namespace library_system.Application.DTOs
{
    public abstract class BaseDto
    {
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}
