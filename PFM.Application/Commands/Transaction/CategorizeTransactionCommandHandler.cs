using MediatR;
using PFM.Domain.Exceptions;
using PFM.Domain.Interfaces;

namespace PFM.Application.Commands.Transaction
{
    public class CategorizeTransactionCommandHandler : IRequestHandler<CategorizeTransactionCommand, Unit>
    {
        private readonly IUnitOfWork _uow;

        public CategorizeTransactionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Unit> Handle(CategorizeTransactionCommand request, CancellationToken cancellationToken)
        {
            // 1. Ensure transaction exists
            var tx = await _uow.Transactions.GetByIdAsync(request.TransactionId, cancellationToken);
            if (tx == null)
                throw new TransactionNotFoundException(request.TransactionId);

            // 2. Ensure category exists
            var category = await _uow.Categories.GetByCodeAsync(request.CatCode, cancellationToken);
            if (category == null)
                throw new CategoryNotFoundException(request.CatCode);

            // 3. Assign and persist
            tx.CatCode = request.CatCode;
            await _uow.CompleteAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
