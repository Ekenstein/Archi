using System.Linq;

namespace Archi.Core
{
    /// <summary>
    /// Provides an abstraction for querying archives in an archive store.
    /// </summary>
    /// <typeparam name="TArchive">The type encapsulating an archive.</typeparam>
    public interface IQueryableArchiveStore<TArchive>
    {
        /// <summary>
        /// Returns an <see cref="IQueryable{T}"/> collection of archives.
        /// </summary>
        IQueryable<TArchive> Archives { get; }
    }
}
