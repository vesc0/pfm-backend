using AutoMapper;
using MediatR;
using PFM.Domain.Entities;
using PFM.Domain.Interfaces;
using PFM.Domain.Exceptions;

namespace PFM.Application.Commands.Transaction
{
    public class SplitTransactionCommandHandler : IRequestHandler<SplitTransactionCommand, Unit>
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public SplitTransactionCommandHandler(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
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

            // Map and assign splits for aggregate-level domain validation
            var splits = _mapper.Map<List<TransactionSplit>>(request.Splits);
            splits.ForEach(s => s.TransactionId = request.TransactionId);

            transaction.Splits = splits;
            transaction.Validate();

            // Remove old splits (if any) and persist new ones
            await _uow.Splits.DeleteByTransactionIdAsync(request.TransactionId, cancellationToken);
            await _uow.Splits.AddRangeAsync(splits, cancellationToken);
            await _uow.CompleteAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
