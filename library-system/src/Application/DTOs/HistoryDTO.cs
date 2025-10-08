using System;
using library_system.Application.DTOs.HATEOAS;

namespace library_system.Application.DTOs
{
    public class LoanDTO : BaseDto
    {
        public Guid LoanId { get; set; }
        public Guid BookId { get; set; }
        public Guid MemberId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int Status { get; set; }
        public decimal? LateFee { get; set; }
        public string Notes { get; set; }
    }
}

