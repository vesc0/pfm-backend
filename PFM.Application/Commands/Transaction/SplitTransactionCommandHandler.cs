using MediatR;
using PFM.Domain.Entities;
using PFM.Domain.Interfaces;
using PFM.Domain.Exceptions;

namespace PFM.Application.Commands.Transaction
{
    public class SplitTransactionCommandHandler : IRequestHandler<SplitTransactionCommand, Unit>
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly ITransactionSplitRepository _splitRepo;

        public SplitTransactionCommandHandler(
            ITransactionRepository transactionRepo,
            ICategoryRepository categoryRepo,
            ITransactionSplitRepository splitRepo)
        {
            _transactionRepo = transactionRepo;
            _categoryRepo = categoryRepo;
            _splitRepo = splitRepo;
        }

        public async Task<Unit> Handle(SplitTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _transactionRepo.GetByIdAsync(request.TransactionId, cancellationToken);
            if (transaction == null)
                throw new TransactionNotFoundException(request.TransactionId);

            var allCodes = request.Splits.Select(s => s.CatCode).Distinct().ToList();
            var foundCodes = await _categoryRepo.ExistsAsync(allCodes, cancellationToken);
            if (!foundCodes)
                throw new BusinessRuleException("category-not-found", "Category not found.", "One or more categories do not exist.");

            var splitSum = request.Splits.Sum(s => s.Amount);
            if (splitSum != transaction.Amount)
                throw new SplitAmountException();

            // 4. Remove old splits (if any)
            await _splitRepo.DeleteByTransactionIdAsync(request.TransactionId, cancellationToken);

            // 5. Add new splits
            var splits = request.Splits.Select(s => new TransactionSplit
            {
                TransactionId = request.TransactionId,
                Amount = s.Amount,
                CatCode = s.CatCode
            }).ToList();

            await _splitRepo.AddRangeAsync(splits, cancellationToken);
            await _splitRepo.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
