using library_system.Application.DTOs;
using library_system.Application.DTOs.Pagination;
using library_system.Domain.Entity;
using library_system.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library_system.Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly IRepository<Loan> _loanRepository;

        public LoanService(IRepository<Loan> loanRepository)
        {
            _loanRepository = loanRepository;
        }

        public async Task<IEnumerable<HistoryDTO>> GetAllLoansAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            return loans.Select(l => new HistoryDTO
            {
                LoanId = l.LoanId,
                BookId = l.BookId,
                MemberId = l.MemberId,
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                ReturnDate = l.ReturnDate,
                Status = (int)l.Status,
                LateFee = l.LateFee,
                Notes = l.Notes
            });
        }

        public async Task<HistoryDTO> GetLoanByIdAsync(Guid id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null) return null;

            return new HistoryDTO
            {
                LoanId = loan.LoanId,
                BookId = loan.BookId,
                MemberId = loan.MemberId,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = (int)loan.Status,
                LateFee = loan.LateFee,
                Notes = loan.Notes
            };
        }

        public async Task<HistoryDTO> CreateLoanAsync(HistoryDTO loanDto)
        {
            var loan = new Loan
            {
                LoanId = Guid.NewGuid(),
                BookId = loanDto.BookId,
                MemberId = loanDto.MemberId,
                LoanDate = loanDto.LoanDate,
                DueDate = loanDto.DueDate,
                ReturnDate = loanDto.ReturnDate,
                Status = (LoanStatus)loanDto.Status,
                LateFee = loanDto.LateFee,
                Notes = loanDto.Notes
            };
            await _loanRepository.AddAsync(loan);

            return new HistoryDTO
            {
                LoanId = loan.LoanId,
                BookId = loan.BookId,
                MemberId = loan.MemberId,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = (int)loan.Status,
                LateFee = loan.LateFee,
                Notes = loan.Notes
            };
        }

        public async Task UpdateLoanAsync(Guid id, HistoryDTO loanDto)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null) throw new KeyNotFoundException("Loan not found.");

            loan.BookId = loanDto.BookId;
            loan.MemberId = loanDto.MemberId;
            loan.LoanDate = loanDto.LoanDate;
            loan.DueDate = loanDto.DueDate;
            loan.ReturnDate = loanDto.ReturnDate;
            loan.Status = (LoanStatus)loanDto.Status;
            loan.LateFee = loanDto.LateFee;
            loan.Notes = loanDto.Notes;

            await _loanRepository.UpdateAsync(loan);
        }

        public async Task DeleteLoanAsync(Guid id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null) throw new KeyNotFoundException("Loan not found.");

            await _loanRepository.DeleteAsync(loan.LoanId);
        }

        public async Task<PagedListDto<HistoryDTO>> GetPagedLoansAsync(PaginationParams paginationParams)
        {
            var loans = _loanRepository.GetAllAsQueryable();
            var pagedLoans = PagedListDto<Loan>.ToPagedList(loans, paginationParams.PageNumber, paginationParams.PageSize);

            var loanDtos = pagedLoans.Items.Select(l => new HistoryDTO
            {
                LoanId = l.LoanId,
                BookId = l.BookId,
                MemberId = l.MemberId,
                LoanDate = l.LoanDate,
                DueDate = l.DueDate,
                ReturnDate = l.ReturnDate,
                Status = (int)l.Status,
                LateFee = l.LateFee,
                Notes = l.Notes
            }).ToList();

            return new PagedListDto<HistoryDTO>(loanDtos, pagedLoans.TotalCount, pagedLoans.CurrentPage, pagedLoans.PageSize);
        }
    }
}
