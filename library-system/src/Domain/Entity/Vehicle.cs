using library_system.Domain.Interfaces;

namespace library_system.Domain.Entity
{
    public class Book
    {
        public Guid BookId { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public int PublicationYear { get; set; }
        public string Category { get; set; } = string.Empty;
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public BookStatus Status { get; set; }

        public Book()
        {
            Status = BookStatus.Available;
        }

        public Book(Guid id, string isbn, string title, string author, string publisher,
                   int publicationYear, string category, int totalCopies)
        {
            BookId = id;
            ISBN = isbn;
            Title = title;
            Author = author;
            Publisher = publisher;
            PublicationYear = publicationYear;
            Category = category;
            TotalCopies = totalCopies;
            AvailableCopies = totalCopies;
            Status = BookStatus.Available;
        }

        public bool IsAvailable()
        {
            return AvailableCopies > 0 && Status == BookStatus.Available;
        }

        public void BorrowCopy()
        {
            if (AvailableCopies <= 0)
                throw new InvalidOperationException("Não há cópias disponíveis para empréstimo");

            AvailableCopies--;

            if (AvailableCopies == 0)
                Status = BookStatus.Borrowed;
        }

        public void ReturnCopy()
        {
            if (AvailableCopies >= TotalCopies)
                throw new InvalidOperationException("Todas as cópias já foram devolvidas");

            AvailableCopies++;

            if (AvailableCopies > 0)
                Status = BookStatus.Available;
        }
    }

    public enum BookStatus
    {
        Available = 0,
        Borrowed = 1,
        Maintenance = 2,
        Lost = 3
    }
}
