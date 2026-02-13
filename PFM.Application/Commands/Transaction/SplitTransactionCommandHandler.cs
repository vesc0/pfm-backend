using MediatR;
using PFM.Domain.Entities;
using PFM.Domain.Interfaces;
using PFM.Domain.Exceptions;

namespace PFM.Application.Commands.Transaction
{
    public class SplitTransactionCommandHandler : IRequestHandler<SplitTransactionCommand, Unit>
    {
        private readonly IUnitOfWork _uow;

        public SplitTransactionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Unit> Handle(SplitTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _uow.Transactions.GetByIdAsync(request.TransactionId, cancellationToken);
            if (transaction == null)
                throw new TransactionNotFoundException(request.TransactionId);

            var allCodes = request.Splits.Select(s => s.CatCode).Distinct().ToList();
            var foundCodes = await _uow.Categories.ExistsAsync(allCodes, cancellationToken);
            if (!foundCodes)
                throw new BusinessRuleException("category-not-found", "Category not found.", "One or more categories do not exist.");

            var splitSum = request.Splits.Sum(s => s.Amount);
            if (splitSum != transaction.Amount)
                throw new SplitAmountException();

            // 4. Remove old splits (if any)
            await _uow.Splits.DeleteByTransactionIdAsync(request.TransactionId, cancellationToken);

            // 5. Add new splits
            var splits = request.Splits.Select(s => new TransactionSplit
            {
                TransactionId = request.TransactionId,
                Amount = s.Amount,
                CatCode = s.CatCode
            }).ToList();

            await _uow.Splits.AddRangeAsync(splits, cancellationToken);
            await _uow.CompleteAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
