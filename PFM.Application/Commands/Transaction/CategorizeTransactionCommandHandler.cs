using MediatR;
using PFM.Domain.Exceptions;
using PFM.Domain.Interfaces;

namespace PFM.Application.Commands.Transaction
{
    public class CategorizeTransactionCommandHandler : IRequestHandler<CategorizeTransactionCommand, Unit>
    {
        private readonly ITransactionRepository _txRepo;
        private readonly ITransactionReadRepository _txReadRepo;
        private readonly ICategoryReadRepository _catReadRepo;

        public CategorizeTransactionCommandHandler(
            ITransactionRepository txRepo,
            ITransactionReadRepository txReadRepo,
            ICategoryReadRepository catReadRepo)
        {
            _txRepo = txRepo;
            _txReadRepo = txReadRepo;
            _catReadRepo = catReadRepo;
        }

        public async Task<Unit> Handle(CategorizeTransactionCommand request, CancellationToken cancellationToken)
        {
            // 1. Ensure transaction exists
            var tx = await _txReadRepo.GetByIdAsync(request.TransactionId, cancellationToken);
            if (tx == null)
                throw new TransactionNotFoundException(request.TransactionId);

            // 2. Ensure category exists
            var category = await _catReadRepo.GetByCodeAsync(request.CatCode, cancellationToken);
            if (category == null)
                throw new CategoryNotFoundException(request.CatCode);

            // 3. Assign and persist
            tx.CatCode = request.CatCode;
            await _txRepo.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
