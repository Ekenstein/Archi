using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Monadicsh;

namespace Archi.Core
{
    /// <summary>
    /// Provides an abstraction of a store that can manage tags of an archive.
    /// </summary>
    /// <typeparam name="TArchive">The type encapsulating an archive.</typeparam>
    public interface IArchiveTagStore<TArchive> : IArchiveStore<TArchive>
    {
        /// <summary>
        /// Returns a collection of zero or more tags associated with the given
        /// <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to retrieve the associated tags from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
        /// a collection of zero or more tags associated with the given <paramref name="archive"/>.
        /// </returns>
        Task<IList<string>> GetTagsAsync(TArchive archive, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Returns a flag indicating whether the <paramref name="archive"/> is associated
        /// with the given <paramref name="tag"/> or not.
        /// </summary>
        /// <param name="archive">The archive to check whether it is associated with the tag or not.</param>
        /// <param name="tag">The tag to check whether it is associated with the archive or not.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
        /// containing a flag indicating whether the <paramref name="tag"/> is associated with the
        /// <paramref name="archive"/> or not.
        /// </returns>
        Task<bool> HasTagAsync(TArchive archive, string tag, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds the <paramref name="tags"/> to the <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to add the tag to.</param>
        /// <param name="tags">The tags to be added to the archive.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
        /// the <see cref="Result"/> of the operation.
        /// </returns>
        Task<Result> AddTagsAsync(TArchive archive, IEnumerable<string> tag, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Removes the <paramref name="tag"/> from the <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to remove the tag from.</param>
        /// <param name="tag">The tag to be removed from the archive.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
        /// the <see cref="Result"/> of the operation.
        /// </returns>
        Task<Result> RemoveTagAsync(TArchive archive, string tag, CancellationToken cancellationToken = default(CancellationToken));
    }
}
