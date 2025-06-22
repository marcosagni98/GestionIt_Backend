namespace Domain.Interfaces.Repositories;

/// <summary>
/// Defines the contract for the Unit of Work pattern, coordinating the writing of changes and transaction management in the database.
/// Allows to start, commit, and rollback transactions, as well as save changes made in the context.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Asynchronously saves all pending changes in the database context.
    /// </summary>
    /// <returns>A task representing the asynchronous save operation.</returns>
    Task SaveAsync();

    /// <summary>
    /// Begins a new database transaction asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous begin transaction operation.</returns>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commits the current transaction asynchronously, applying all changes made during the transaction.
    /// </summary>
    /// <returns>A task representing the asynchronous commit operation.</returns>
    Task CommitTransactionAsync();

    /// <summary>
    /// Rolls back the current transaction asynchronously, reverting all changes made during the transaction.
    /// </summary>
    /// <returns>A task representing the asynchronous rollback operation.</returns>
    Task RollbackTransactionAsync();

    /// <summary>
    /// Indicates whether a transaction is currently active.
    /// </summary>
    bool IsTransactionActive { get; }
}