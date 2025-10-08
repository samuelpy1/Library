using System;
using library_system.Application.DTOs.HATEOAS;

namespace library_system.Application.DTOs
{
    public class BookDTO : BaseDto
    {
        public Guid BookId { get; set; }
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public int PublicationYear { get; set; }
        public string Category { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public int Status { get; set; }
    }
}

