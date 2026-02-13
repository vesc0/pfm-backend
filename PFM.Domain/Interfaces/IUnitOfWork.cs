namespace PFM.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ITransactionRepository Transactions { get; }
        ICategoryRepository Categories { get; }
        ITransactionSplitRepository Splits { get; }

        Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    }
}
