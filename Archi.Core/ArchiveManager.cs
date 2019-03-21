using Fileicsh.Abstraction;
using Fileicsh.Abstraction.Extensions;
using Monadicsh;
using Monadicsh.Extensions;
using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Archi.Core
{
    /// <summary>
    /// Provides the APIs for managing archives in a persistance store.
    /// </summary>
    /// <typeparam name="TArchive">The type encapsulating an archive.</typeparam>
    public class ArchiveManager<TArchive> : IDisposable
    {
        private bool _disposed;

        /// <summary>
        /// The persistance store for managing the archives.
        /// </summary>
        protected internal IArchiveStore<TArchive> Store { get; }

        /// <summary>
        /// The storage containing all the files associated with the archives.
        /// </summary>
        protected internal IStorage Storage { get; }

        /// <summary>
        /// The options for managing archives.
        /// </summary>
        protected internal ArchiveOptions Options { get; }

        /// <summary>
        /// The validators for validating archives.
        /// </summary>
        protected IList<IArchiveValidator<TArchive>> Validators { get; } = new List<IArchiveValidator<TArchive>>();

        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public ArchiErrorDescriber ErrorDescriber { get; }

        public ArchiveManager(IArchiveStore<TArchive> store, 
            IStorage storage, 
            ArchiveOptions options, 
            IEnumerable<IArchiveValidator<TArchive>> validators,
            IServiceProvider serviceProvider,
            ArchiErrorDescriber errorDescriber = null)
        {
            Store = store ?? throw new ArgumentNullException(nameof(store));
            Storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Options = options ?? throw new ArgumentNullException(nameof(options));
            ErrorDescriber = errorDescriber ?? new ArchiErrorDescriber();

            if (validators != null)
            {
                foreach (var validator in validators)
                {
                    Validators.Add(validator);
                }
            }
        }

        /// <summary>
        /// Returns a flag indicating whether the archive supports files or not.
        /// </summary>
        /// <value>
        /// Returns true if the archive supports files, otherwise false.
        /// </value>
        public virtual bool SupportsArchiveFiles => Store is IArchiveFileStore<TArchive>;

        /// <summary>
        /// Returns a flag indicating whether the archive supports tags or not.
        /// </summary>
        /// <value>
        /// Returns true if the archive supports tags, otherwise false.
        /// </value>
        public virtual bool SupportsArchiveTags => Store is IArchiveTagStore<TArchive>;

        /// <summary>
        /// Returns a flag indicating whether the archive store supports returning
        /// an <see cref="IQueryable{T}"/> collection of archives.
        /// </summary>
        /// <value>
        /// True if the backing archive store supports returning <see cref="IQueryable{T}"/>
        /// collection of archives, otherwise false.
        /// </value>
        public virtual bool SupportsQueryableArchives => Store is IQueryableArchiveStore<TArchive>;

        /// <summary>
        /// Returns a an <see cref="IQueryable{T}"/> collection of archives.
        /// </summary>
        /// <value>
        /// Returns a collection of zero or more archives that can be queried.
        /// </value>
        /// <exception cref="NotSupportedException">If the underlying store does not implement <see cref="IQueryableArchiveStore{TArchive}"/>.</exception>
        public virtual IQueryable<TArchive> Archives => Maybe
            .Create(Store as IQueryableArchiveStore<TArchive>)
            .Coalesce(store => store.Archives)
            .OrThrow(() => new NotSupportedException(ArchiResources.StoreNotIQueryableArchiveStore));

        /// <summary>
        /// Returns the archive corresponding to the given <paramref name="id"/>.
        /// If there was no archive corresponding to the given <paramref name="id"/>,
        /// <see cref="Maybe{T}.Nothing"/> will be returned, otherwise <see cref="Maybe{T}.Just(T)"/>.
        /// </summary>
        /// <param name="id">The id of the archive.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> containing either <see cref="Maybe{T}.Nothing"/> or <see cref="Maybe{T}.Just(T)"/>
        /// containing the archive.
        /// </returns>
        /// <exception cref="ObjectDisposedException">If the manager is disposed.</exception>
        public virtual Task<Maybe<TArchive>> FindByIdAsync(string id)
        {
            ThrowIfDisposed();
            return Store.FindByIdAsync(id, CancellationToken);
        }

        /// <summary>
        /// Returns the string representation of the id of the given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to retrieve the id from.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> containing the id of the given <paramref name="archive"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the manager is disposed.</exception>
        public virtual Task<string> GetIdAsync(TArchive archive)
        {
            ThrowIfDisposed();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            return Store.GetIdAsync(archive);
        }

        /// <summary>
        /// Creates the given <paramref name="archive"/> in the archive store.
        /// </summary>
        /// <param name="archive">The archive to create.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation, containing the <see cref="Result"/> of the create operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        public virtual async Task<Result> CreateAsync(TArchive archive)
        {
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            var result = await ValidateAsync(archive);
            if (!result.Succeeded)
            {
                return result;
            }

            return await Store.CreateAsync(archive, CancellationToken);
        }

        /// <summary>
        /// Updates the given <paramref name="archive"/> in the archive store.
        /// </summary>
        /// <param name="archive">The archive to update.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation, containing the <see cref="Result"/> of the update operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        public virtual Task<Result> UpdateAsync(TArchive archive)
        {
            ThrowIfDisposed();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            return UpdateArchiveAsync(archive);
        }

        /// <summary>
        /// Deletes the given <paramref name="archive"/> in the archive store.
        /// </summary>
        /// <param name="archive">The archive to delete.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the asynchronous operation, containing the <see cref="Result"/> of the delete operation.
        /// </returns>
        public virtual Task<Result> DeleteAsync(TArchive archive)
        {
            ThrowIfDisposed();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            return Store.DeleteAsync(archive, CancellationToken);
        }

        /// <summary>
        /// Returns a collection of files associated with the given <paramref name="archive"/>.
        /// The collection of files always contains zero or more elements.
        /// The <paramref name="archive"/> must support files.
        /// </summary>
        /// <param name="archive">The archive to retrieve zero or more files from.</param>
        /// <returns>
        /// An <see cref="IAsyncEnumerable{T}"/> containing zero or more <see cref="IFile"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="NotSupportedException">If <see cref="SupportsArchiveFiles"/> is false.</exception>
        /// <exception cref="ObjectDisposedException">If the manager is disposed.</exception>
        public virtual IAsyncEnumerable<IFile> GetFilesAsync(TArchive archive)
        {
            ThrowIfDisposed();
            
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            var store = GetFileStore();

            return new AsyncEnumerable<IFile>(async yield =>
            {
                var files = await store.GetFilesAsync(archive, yield.CancellationToken);
                foreach (var info in files)
                {
                    var file = await GetArchiveFileAsync(archive, info, yield.CancellationToken);
                    if (file == null)
                    {
                        continue;
                    }

                    await yield.ReturnAsync(file);
                }
            });
        }

        /// <summary>
        /// Returns a file associated with the given <paramref name="archive"/> that
        /// has the given <paramref name="fileName"/>.
        /// </summary>
        /// <param name="archive">The archive the file is associated with.</param>
        /// <param name="fileName">The name of the file to retrieve.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
        /// a <see cref="Maybe{T}"/> that will either contain the file corresponding to the given <paramref name="fileName"/>
        /// or <see cref="Maybe{T}.Nothing"/>.
        /// </returns>
        public virtual async Task<Maybe<IFile>> GetFileByNameAsync(TArchive archive, string fileName)
        {
            ThrowIfDisposed();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            var store = GetFileStore();
            var fileInfo = await store.GetFileByNameAsync(archive, fileName, CancellationToken);
            if (fileInfo.IsNothing)
            {
                return Maybe<IFile>.Nothing;
            }

            var storage = await GetArchiveStorageAsync(archive);
            return Maybe.Create(await storage.GetFileAsync(fileInfo.Value, Options.StorageOptions.Tag, CancellationToken));
        }

        /// <summary>
        /// Adds the given <paramref name="file"/> to the given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to create the file for.</param>
        /// <param name="file">The file to associate with the archive.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> containing the <see cref="Result"/> of operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> or <paramref name="file"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the manager has been disposed.</exception>
        /// <exception cref="NotSupportedException">If the archive doesn't support files.</exception>
        public virtual async Task<Result> AddFileAsync(TArchive archive, IFile file)
        {
            ThrowIfDisposed();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var store = GetFileStore();
            var renamedFile = file.Rename(Guid.NewGuid().ToString(), true);

            var result = await store.AddFileAsync(archive, renamedFile, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            if (!await CreateArchiveFileAsync(archive, renamedFile))
            {
                return ErrorDescriber.FailedToCreateFile(file.FileName);
            }

            return await UpdateArchiveAsync(archive);
        }

        /// <summary>
        /// Removes the given <paramref name="file"/> from the given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to remove the file from.</param>
        /// <param name="file">The file to be removed from the archive.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
        /// the <see cref="Result"/> of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> or <paramref name="file"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the manager has been disposed.</exception>
        /// <exception cref="NotSupportedException">If the archive doesn't support files.</exception>
        public virtual async Task<Result> RemoveFileAsync(TArchive archive, IFileInfo file)
        {
            ThrowIfDisposed();
            var store = GetFileStore();

            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var result = await store.RemoveFileAsync(archive, file, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            if (!await RemoveArchiveFileAsync(archive, file))
            {
                return ErrorDescriber.FailedToRemoveFile(file.FileName);
            }

            return await UpdateArchiveAsync(archive);
        }

        /// <summary>
        /// Returns a collection of zero or more tags associated with the
        /// given <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to retrieve the associated tags from.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
        /// containing a collection of zero or more tags associated with the <paramref name="archive"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the manager is disposed.</exception>
        /// <exception cref="NotSupportedException">If the archive does not support tags.</exception>
        public virtual Task<IList<string>> GetTagsAsync(TArchive archive)
        {
            ThrowIfDisposed();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            var store = GetTagStore();
            return store.GetTagsAsync(archive, CancellationToken);
        }

        /// <summary>
        /// Returns a flag indicating whether the <paramref name="tag"/> is associated
        /// with the <paramref name="archive"/> or not.
        /// </summary>
        /// <param name="archive">The archive to check whether it is associated with the tag or not.</param>
        /// <param name="tag">The tag to check whether it is associated with the archive or not.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
        /// a flag indicating whether the <paramref name="tag"/> is associated with the <paramref name="archive"/>
        /// or not.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the manager is disposed.</exception>
        /// <exception cref="NotSupportedException">If the archive does not support tags.</exception>
        public virtual Task<bool> HasTagAsync(TArchive archive, string tag)
        {
            ThrowIfDisposed();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            var store = GetTagStore();
            return store.HasTagAsync(archive, tag, CancellationToken);
        }

        /// <summary>
        /// Adds the given <paramref name="tags"/> to the <paramref name="archive"/> iff
        /// the <paramref name="tag"/> isn't already associated with the <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to add the tag to.</param>
        /// <param name="tag">The tag to be added to the archive.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
        /// the <see cref="Result"/> of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> or <paramref name="tags"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the manager is disposed.</exception>
        /// <exception cref="NotSupportedException">If the archive does not support tags.</exception>
        public virtual async Task<Result> AddTagsAsync(TArchive archive, IEnumerable<string> tags)
        {
            ThrowIfDisposed();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            var store = GetTagStore();

            if (tags == null)
            {
                throw new ArgumentNullException(nameof(tags));
            }
            
            var result = await store.AddTagsAsync(archive, tags, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return await UpdateArchiveAsync(archive);
        }

        /// <summary>
        /// Removes the <paramref name="tag"/> from the <paramref name="archive"/> iff
        /// the <paramref name="tag"/> is associated with the <paramref name="archive"/>.
        /// </summary>
        /// <param name="archive">The archive to remove the tag from.</param>
        /// <param name="tag">The tag to be removed from the archive.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation, containing
        /// the <see cref="Result"/> of the operation.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="archive"/> is null.</exception>
        /// <exception cref="ObjectDisposedException">If the manager is disposed.</exception>
        /// <exception cref="NotSupportedException">If the archive does not support tags.</exception>
        public virtual async Task<Result> RemoveTagAsync(TArchive archive, string tag)
        {
            ThrowIfDisposed();
            if (archive == null)
            {
                throw new ArgumentNullException(nameof(archive));
            }

            var store = GetTagStore();
            if (!await store.HasTagAsync(archive, tag))
            {
                return ErrorDescriber.TagDoesNotExist(tag);
            }

            var result = await store.RemoveTagAsync(archive, tag, CancellationToken);
            if (!result.Succeeded)
            {
                return result;
            }

            return await UpdateArchiveAsync(archive);
        }

        private async Task<Result> ValidateAsync(TArchive archive)
        {
            var result = Result.Success;
            foreach (var v in Validators)
            {
                result = result.And(await v.ValidateAsync(this, archive));
            }

            return result;
        }

        private async Task<Result> UpdateArchiveAsync(TArchive archive)
        {
            var result = await ValidateAsync(archive);
            if (!result.Succeeded)
            {
                return result;
            }

            return await Store.UpdateAsync(archive, CancellationToken);
        }

        private async Task<bool> RemoveArchiveFileAsync(TArchive archive, IFileInfo file)
        {
            var storage = await GetArchiveStorageAsync(archive);
            return await storage.DeleteFileAsync(file, Options.StorageOptions.Tag, CancellationToken);
        }

        private async Task<bool> CreateArchiveFileAsync(TArchive archive, IFile file)
        {
            var storage = await GetArchiveStorageAsync(archive);
            return await storage.CreateFileAsync(file, Options.StorageOptions.Tag, CancellationToken);
        }

        private async Task<IFile> GetArchiveFileAsync(TArchive archive, IFileInfo file, CancellationToken cancellationToken)
        {
            var storage = await GetArchiveStorageAsync(archive);
            return await storage.GetFileAsync(file, Options.StorageOptions.Tag, cancellationToken);
        }

        private async Task<IStorage> GetArchiveStorageAsync(TArchive archive) => Storage.Prefix(await GetIdAsync(archive));

        private IArchiveFileStore<TArchive> GetFileStore() => Maybe
            .Create(Store as IArchiveFileStore<TArchive>)
            .OrThrow(() => new NotSupportedException(ArchiResources.StoreNotIArchiveFileStore));

        private IArchiveTagStore<TArchive> GetTagStore() => Maybe
            .Create(Store as IArchiveTagStore<TArchive>)
            .OrThrow(() => new NotSupportedException(ArchiResources.StoreNotIArchiveTagStore));

        /// <summary>
        /// Throws <see cref="ObjectDisposedException"/> if the manager is disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">If the manager is disposed.</exception>
        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Disposes the manager.
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
                    Store.Dispose();
                    Storage.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
