﻿using Fileicsh.Abstraction;
using Monadicsh;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Archi.Core
{
    /// <summary>
    /// Provides an abstraction for a store that manages files associated with an archive.
    /// </summary>
    /// <typeparam name="TArchive">Type encapsulating an archive.</typeparam>
    public interface IArchiveFileStore<TArchive> : IArchiveStore<TArchive>
    {
        /// <summary>
        /// Returns a collection of files associated with the given <paramref name="archive"/>.
        /// The collection always contains zero or more elements.
        /// </summary>
        /// <param name="archive">The archive to retrieve the files from.</param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> containing the collection of zero or more files
        /// associated with the given <paramref name="archive"/>.
        /// </returns>
        Task<IList<IFileInfo>> GetFilesAsync(TArchive archive, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Adds the given <paramref name="file"/> to the the given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to create the file for.</param>
        /// <param name="file">The file to associate with the archive.</param>
        /// <param name="cancellationToken">
        /// The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.
        /// </param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> containing the <see cref="Result"/> of operation.
        /// </returns>
        Task<Result> AddFileAsync(TArchive archive, IFileInfo file, CancellationToken cancellationToken = default(CancellationToken));
    }
}
