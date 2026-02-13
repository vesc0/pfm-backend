using Microsoft.EntityFrameworkCore;
using PFM.Domain.Exceptions;
using PFM.Domain.Interfaces;
using PFM.Infrastructure.Persistence.Repositories;

namespace PFM.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private ITransactionRepository? _transactions;
        private ICategoryRepository? _categories;
        private ITransactionSplitRepository? _splits;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public ITransactionRepository Transactions
        {
            get
            {
                _transactions ??= new TransactionRepository(_context);
                return _transactions;
            }
        }

        public ICategoryRepository Categories
        {
            get
            {
                _categories ??= new CategoryRepository(_context);
                return _categories;
            }
        }

        public ITransactionSplitRepository Splits
        {
            get
            {
                _splits ??= new TransactionSplitRepository(_context);
                return _splits;
            }
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new BusinessRuleException(
                    "concurrency-conflict",
                    message: "The record was modified by another user.",
                    details: ex.Message);
            }
            catch (DbUpdateException dbEx)
            {
                var inner = dbEx.InnerException;
                var isPgDup =
                    inner?.GetType().Name == "PostgresException" &&
                    (inner.GetType().GetProperty("SqlState")?.GetValue(inner) as string) == "23505";

                if (isPgDup)
                {
                    throw new BusinessRuleException(
                        "duplicate-key",
                        message: "A record with the same key already exists.",
                        details: "One or more entity keys already exist in the database.");
                }

                throw;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
