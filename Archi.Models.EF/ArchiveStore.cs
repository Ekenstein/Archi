using Archi.Core;
using Fileicsh.Abstraction;
using Microsoft.EntityFrameworkCore;
using Monadicsh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Monadicsh.Extensions;

namespace Archi.Models.EF
{
    /// <summary>
    /// An archive store that persists <see cref="Archive"/> with a <see cref="DbContext"/>.
    /// </summary>
    public class ArchiveStore : IQueryableArchiveStore<Archive>, 
        IArchiveFileStore<Archive>, 
        IArchiveTagStore<Archive>
    {
        private bool _disposed;

        /// <summary>
        /// The underlying <see cref="DbContext"/> persisting the archives.
        /// </summary>
        public DbContext Context { get; }
        public ArchiErrorDescriber ErrorDescriber { get; }

        public ArchiveStore(DbContext context, ArchiErrorDescriber errorDescriber = null)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            ErrorDescriber = errorDescriber ?? new ArchiErrorDescriber();
        }

        public IQueryable<Archive> Archives => Context.Set<Archive>();

        /// <summary>
        /// Saves the given <paramref name="archive"/> to the underlying <see cref="DbContext"/>.
        /// </summary>
        /// <param name="archive">The archive to persist in the <see cref="DbContext"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the <see cref="Result"/> of the create operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the store is disposed.</exception>
        /// <exception cref="OperationCanceledException">If cancellation was requested through the <paramref name="cancellationToken"/>.</exception>
        public async Task<Result> CreateAsync(Archive archive, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            Context.Add(archive);
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }

        /// <summary>
        /// Adds the given <paramref name="file"/> to the given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to associate the given <paramref name="file"/> with.</param>
        /// <param name="file">The file to add to the given <paramref name="archive"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the <see cref="Result"/> of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> or <paramref name="file"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the store is disposed.</exception>
        /// <exception cref="OperationCanceledException">If cancellation was requested through the <paramref name="cancellationToken"/>.</exception>
        public Task<Result> AddFileAsync(Archive archive, IFileInfo file, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            archive.Files.Add(new ArchiveFile
            {
                File = new File
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType
                }
            });

            return Task.FromResult(Result.Success);
        }

        /// <summary>
        /// Deletes the given <paramref name="archive"/> from the underlying <see cref="DbContext"/>.
        /// </summary>
        /// <param name="archive">The archive to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the <see cref="Result"/> of the delete operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the store is disposed.</exception>
        /// <exception cref="OperationCanceledException">If cancellation was requested through the <paramref name="cancellationToken"/>.</exception>
        public async Task<Result> DeleteAsync(Archive archive, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            Context.Remove(archive);
            await Context.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }

        /// <summary>
        /// Returns an <see cref="Archive"/> corresponding to the given <paramref name="id"/>,
        /// if it exists.
        /// </summary>
        /// <param name="id">The string representation of the ID.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing either Nothing or the archive
        /// corresponding to the given <paramref name="id"/>.
        /// </returns>
        /// <exception cref="ObjectDisposedException">If the store is disposed.</exception>
        /// <exception cref="OperationCanceledException">If cancellation was requested through the <paramref name="cancellationToken"/>.</exception>
        public async Task<Maybe<Archive>> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (!Guid.TryParse(id, out var guid))
            {
                return Maybe<Archive>.Nothing;
            }

            return await Archives.SingleOrDefaultAsync(a => a.Id == guid, cancellationToken);
        }

        /// <summary>
        /// Returns all the files associated with the given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to retrieve all the associated files from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing an <see cref="IEnumerable{T}"/> of zero
        /// or more <see cref="IFileInfo"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the store is disposed.</exception>
        /// <exception cref="OperationCanceledException">If cancellation was requested through the <paramref name="cancellationToken"/>.</exception>
        public async Task<IList<IFileInfo>> GetFilesAsync(Archive archive, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            var files = await Context
                .Set<ArchiveFile>()
                .Where(a => a.ArchiveId == archive.Id)
                .Select(a => a.File)
                .ToListAsync(cancellationToken);

            return files
                .Select(f => new FileInfo(f.FileName, f.ContentType))
                .ToArray();
        }

        /// <summary>
        /// Returns a string representation of the id of the given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to retrieve id from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the string
        /// representation of the id of the given <paramref name="archive"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the store is disposed.</exception>
        /// <exception cref="OperationCanceledException">If cancellation was requested through the <paramref name="cancellationToken"/>.</exception>
        public Task<string> GetIdAsync(Archive archive, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            return Task.FromResult(archive.Id.ToString());
        }

        /// <summary>
        /// Updates the given <paramref name="archive"/> to the <see cref="DbContext"/>.
        /// </summary>
        /// <param name="archive">The archive to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the <see cref="Result"/> of the update operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the store is disposed.</exception>
        /// <exception cref="OperationCanceledException">If cancellation was requested through <paramref name="cancellationToken"/>.</exception>
        public async Task<Result> UpdateAsync(Archive archive, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            Context.Attach(archive);
            Context.Update(archive);
            await Context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }

        /// <summary>
        /// Throws <see cref="ObjectDisposedException"/> if the store has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Disposes the store and its underlying <see cref="DbContext"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    Context.Dispose();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Returns a collection of zero or more tags associated with the given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to fetch the associated tags from.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
        /// an <see cref="IList{T}"/> of zero or more tags associated with the <paramref name="archive"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the <see cref="DbContext"/> has been disposed.</exception>
        /// <exception cref="OperationCanceledException">If cancellation was requested through the <paramref name="cancellationToken"/>.</exception>
        public async Task<IList<string>> GetTagsAsync(Archive archive, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            return await Context
                .Set<ArchiveTag>()
                .Where(a => a.ArchiveId == archive.Id)
                .Select(a => a.Tag.Name)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Returns a flag indicating whether the given <paramref name="tag"/> is associated
        /// with the given <paramref name="archive"/> or not.
        /// </summary>
        /// <param name="archive">The archive to check whether it is associated with the tag or not.</param>
        /// <param name="tag">The tag to check whether it is associated with the archive or not.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
        /// a flag indicating whether the <paramref name="tag"/> is associated with the <paramref name="archive"/> or not.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the <see cref="DbContext"/> has been disposed.</exception>
        /// <exception cref="OperationCanceledException">If cancellation was requested through the <paramref name="cancellationToken"/>.</exception>
        public Task<bool> HasTagAsync(Archive archive, string tag, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            return Context
                .Set<ArchiveTag>()
                .Where(a => a.ArchiveId == archive.Id)
                .Select(a => a.Tag.Name)
                .ContainsAsync(tag, cancellationToken);
        }

        /// <summary>
        /// Adds the given <paramref name="tag"/> to the given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to associate the tag with.</param>
        /// <param name="tag">The tag to associate with the archive.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
        /// the <see cref="Result"/> of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the store has been disposed.</exception>
        /// <exception cref="OperationCanceledException">If cancellation has been requested through the <paramref name="cancellationToken"/>.</exception>
        public Task<Result> AddTagAsync(Archive archive, string tag, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            if (string.IsNullOrWhiteSpace(tag))
            {
                return Task.FromResult(Result.Failed(ErrorDescriber.TagMustNotBeNullOrEmpty));
            }

            archive.Tags.Add(new ArchiveTag
            {
                Tag = new Tag
                {
                    Name = tag
                }
            });

            return Task.FromResult(Result.Success);
        }

        /// <summary>
        /// Removes the <paramref name="tag"/> from the given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to remove the tag from.</param>
        /// <param name="tag">The tag to be removed.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the
        /// <see cref="Result"/> of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the store has been disposed.</exception>
        /// <exception cref="OperationCanceledException">If cancellation has been requested through the <paramref name="cancellationToken"/>.</exception>
        public async Task<Result> RemoveTagAsync(Archive archive, string tag, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            var dbTag = await Context
                .Set<ArchiveTag>()
                .SingleOrDefaultAsync(t => t.ArchiveId == archive.Id && t.Tag.Name == tag, cancellationToken);

            if (dbTag != null)
            {
                Context.Remove(dbTag);
            }

            return Result.Success;
        }

        public async Task<Maybe<IFileInfo>> GetFileByNameAsync(Archive archive, string fileName, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            cancellationToken.ThrowIfCancellationRequested();

            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            var file = await Context
                .Set<ArchiveFile>()
                .Where(f => f.ArchiveId == archive.Id)
                .Select(f => f.File)
                .Where(f => f.FileName == fileName)
                .FirstOrDefaultAsync(cancellationToken);

            return Maybe
                .Create(file)
                .Coalesce(f => new FileInfo(f.FileName, f.ContentType))
                .Cast<IFileInfo>();
        }
    }
}
