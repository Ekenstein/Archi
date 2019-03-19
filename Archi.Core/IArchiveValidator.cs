using Monadicsh;
using System.Threading.Tasks;

namespace Archi.Core
{
    /// <summary>
    /// An abstraction of a validator that can validate archives.
    /// </summary>
    /// <typeparam name="TArchive">The type that represents an archive.</typeparam>
    public interface IArchiveValidator<TArchive>
    {
        /// <summary>
        /// Validates the given <paramref name="archive"/> as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="ArchiveManager{TArchive}"/> managing the archive store.</param>
        /// <param name="archive">The archive to validate.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the <see cref="Result"/> of the asynchronous validation.
        /// </returns>
        Task<Result> ValidateAsync(ArchiveManager<TArchive> manager, TArchive archive);
    }
}
