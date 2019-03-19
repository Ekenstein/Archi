using Monadicsh;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Archi.Core
{
    /// <summary>
    /// Provides an abstraction for a store which manges archives.
    /// </summary>
    /// <typeparam name="TArchive">The type encapsulating an archive.</typeparam>
    public interface IArchiveStore<TArchive> : IDisposable
    {
        /// <summary>
        /// Returns the archive corresponding to the given <paramref name="id"/>.
        /// If there was no archive corresponding to the given <paramref name="id"/>,
        /// <see cref="Maybe{T}.Nothing"/> will be returned, otherwise <see cref="Maybe{T}.Just(T)"/>.
        /// </summary>
        /// <param name="id">The id of the archive.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> containing either <see cref="Maybe{T}.Nothing"/> or <see cref="Maybe{T}.Just(T)"/>
        /// containing the archive.
        /// </returns>
        Task<Maybe<TArchive>> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns the string representation of the id of the given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to retrieve the id from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> containing the id of the given <paramref name="archive"/>.
        /// </returns>
        Task<string> GetIdAsync(TArchive archive, CancellationToken cancellationToken = default(CancellationToken));
        
        /// <summary>
        /// Creates the given <paramref name="archive"/> in the archive store.
        /// </summary>
        /// <param name="archive">The archive to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation, containing the <see cref="Result"/> of the create operation.
        /// </returns>
        Task<Result> CreateAsync(TArchive archive, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the given <paramref name="archive"/> in the archive store.
        /// </summary>
        /// <param name="archive">The archive to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation, containing the <see cref="Result"/> of the update operation.
        /// </returns>
        Task<Result> UpdateAsync(TArchive archive, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the given <paramref name="archive"/> in the archive store.
        /// </summary>
        /// <param name="archive">The archive to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation, containing the <see cref="Result"/> of the delete operation.
        /// </returns>
        Task<Result> DeleteAsync(TArchive archive, CancellationToken cancellationToken = default(CancellationToken));
    }
}
