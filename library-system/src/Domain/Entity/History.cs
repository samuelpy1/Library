namespace library_system.Domain.Entity;

public class Loan
{
    public Guid LoanId { get; set; }
    public Guid BookId { get; set; }
    public Guid MemberId { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
    public decimal? LateFee { get; set; }
    public string Notes { get; set; }

    public Loan()
    {
        Status = LoanStatus.Active;
        LoanDate = DateTime.UtcNow;
        DueDate = DateTime.UtcNow.AddDays(14);
    }

    public Loan(Guid loanId, Guid bookId, Guid memberId, int loanDurationDays = 14)
    {
        if (bookId == Guid.Empty)
            throw new ArgumentNullException(nameof(bookId), "Book não pode ser nulo.");
        if (memberId == Guid.Empty)
            throw new ArgumentNullException(nameof(memberId), "Member não pode ser nulo.");

        LoanId = loanId;
        BookId = bookId;
        MemberId = memberId;
        LoanDate = DateTime.UtcNow;
        DueDate = DateTime.UtcNow.AddDays(loanDurationDays);
        Status = LoanStatus.Active;
    }

    public void ReturnBook()
    {
        if (Status == LoanStatus.Returned)
            throw new InvalidOperationException("Este empréstimo já foi devolvido.");

        ReturnDate = DateTime.UtcNow;
        Status = LoanStatus.Returned;

        if (IsLate())
        {
            LateFee = CalculateLateFee();
        }
    }

    public bool IsLate()
    {
        var compareDate = ReturnDate ?? DateTime.UtcNow;
        return compareDate > DueDate;
    }

    public int GetDaysLate()
    {
        if (!IsLate()) return 0;
        var compareDate = ReturnDate ?? DateTime.UtcNow;
        return (compareDate - DueDate).Days;
    }

    public decimal CalculateLateFee()
    {
        if (!IsLate()) return 0;
        var daysLate = GetDaysLate();
        return daysLate * 2.00m; // R$ 2.00 por dia de atraso
    }

    public void RenewLoan(int additionalDays = 14)
    {
        if (Status != LoanStatus.Active)
            throw new InvalidOperationException("Apenas empréstimos ativos podem ser renovados.");

        if (IsLate())
            throw new InvalidOperationException("Empréstimos em atraso não podem ser renovados.");

        DueDate = DueDate.AddDays(additionalDays);
    }

    public void ValidateForSave()
    {
        if (BookId == Guid.Empty)
            throw new ArgumentNullException(nameof(BookId), "Book não pode ser nulo.");
        if (MemberId == Guid.Empty)
            throw new ArgumentNullException(nameof(MemberId), "Member não pode ser nulo.");
        if (LoanDate == default || LoanDate == DateTime.MinValue)
            throw new ArgumentException("A data do empréstimo está em formato inválido.");
        if (DueDate == default || DueDate == DateTime.MinValue)
            throw new ArgumentException("A data de devolução está em formato inválido.");
    }
}

public enum LoanStatus
{
    Active = 0,
    Returned = 1,
    Late = 2,
    Cancelled = 3
}



