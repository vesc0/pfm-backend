using MediatR;
using PFM.Domain.Exceptions;
using PFM.Domain.Interfaces;

namespace PFM.Application.Commands.Transaction
{
    public class CategorizeTransactionCommandHandler : IRequestHandler<CategorizeTransactionCommand, Unit>
    {
        private readonly ITransactionRepository _txRepo;
        private readonly ICategoryRepository _catRepo;

        public CategorizeTransactionCommandHandler(
            ITransactionRepository txRepo,
            ICategoryRepository catRepo)
        {
            _txRepo = txRepo;
            _catRepo = catRepo;
        }

        public async Task<Unit> Handle(
            CategorizeTransactionCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Ensure transaction exists
            var tx = await _txRepo.GetByIdAsync(request.TransactionId, cancellationToken);
            if (tx == null)
                throw new TransactionNotFoundException(request.TransactionId);

            // 2. Ensure category exists
            var category = await _catRepo.GetByCodeAsync(request.CatCode, cancellationToken);
            if (category == null)
                throw new CategoryNotFoundException(request.CatCode);

            // 3. Assign and persist
            tx.CatCode = request.CatCode;
            await _txRepo.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
