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
    public class HistoryService : IHistoryService
    {
        private readonly IRepository<Loan> _historyRepository;

        public HistoryService(IRepository<Loan> historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public async Task<IEnumerable<HistoryDTO>> GetAllHistoriesAsync()
        {
            var histories = await _historyRepository.GetAllAsync();
            return histories.Select(h => new HistoryDTO
            {
                LoanId = h.LoanId,
                BookId = h.BookId,
                MemberId = h.MemberId,
                LoanDate = h.LoanDate,
                DueDate = h.DueDate,
                ReturnDate = h.ReturnDate,
                Status = (int)h.Status,
                LateFee = h.LateFee,
                Notes = h.Notes
            });
        }

        public async Task<HistoryDTO> GetHistoryByIdAsync(Guid id)
        {
            var history = await _historyRepository.GetByIdAsync(id);
            if (history == null) return null;

            return new HistoryDTO
            {
                LoanId = history.LoanId,
                BookId = history.BookId,
                MemberId = history.MemberId,
                LoanDate = history.LoanDate,
                DueDate = history.DueDate,
                ReturnDate = history.ReturnDate,
                Status = (int)history.Status,
                LateFee = history.LateFee,
                Notes = history.Notes
            };
        }

        public async Task<HistoryDTO> CreateHistoryAsync(HistoryDTO historyDto)
        {
            var history = new Loan
            {
                LoanId = Guid.NewGuid(),
                BookId = historyDto.BookId,
                MemberId = historyDto.MemberId,
                LoanDate = historyDto.LoanDate,
                DueDate = historyDto.DueDate,
                ReturnDate = historyDto.ReturnDate,
                Status = (LoanStatus)historyDto.Status,
                LateFee = historyDto.LateFee,
                Notes = historyDto.Notes
            };
            await _historyRepository.AddAsync(history);
            return historyDto;
        }

        public async Task UpdateHistoryAsync(Guid id, HistoryDTO historyDto)
        {
            var history = await _historyRepository.GetByIdAsync(id);
            if (history == null) throw new KeyNotFoundException("History not found.");

            history.BookId = historyDto.BookId;
            history.MemberId = historyDto.MemberId;
            history.LoanDate = historyDto.LoanDate;
            history.DueDate = historyDto.DueDate;
            history.ReturnDate = historyDto.ReturnDate;
            history.Status = (LoanStatus)historyDto.Status;
            history.LateFee = historyDto.LateFee;
            history.Notes = historyDto.Notes;

            await _historyRepository.UpdateAsync(history);
        }

        public async Task DeleteHistoryAsync(Guid id)
        {
            var history = await _historyRepository.GetByIdAsync(id);
            if (history == null) throw new KeyNotFoundException("History not found.");

            await _historyRepository.DeleteAsync(history.LoanId);
        }

        public async Task<PagedListDto<HistoryDTO>> GetPagedHistoriesAsync(PaginationParams paginationParams)
        {
            var histories = _historyRepository.GetAllAsQueryable();
            var pagedHistories = PagedListDto<Loan>.ToPagedList(histories, paginationParams.PageNumber, paginationParams.PageSize);

            var historyDtos = pagedHistories.Items.Select(h => new HistoryDTO
            {
                LoanId = h.LoanId,
                BookId = h.BookId,
                MemberId = h.MemberId,
                LoanDate = h.LoanDate,
                DueDate = h.DueDate,
                ReturnDate = h.ReturnDate,
                Status = (int)h.Status,
                LateFee = h.LateFee,
                Notes = h.Notes
            }).ToList();

            return new PagedListDto<HistoryDTO>(historyDtos, pagedHistories.TotalCount, pagedHistories.CurrentPage, pagedHistories.PageSize);
        }
    }
}

