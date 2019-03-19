namespace Archi.Core
{
    /// <summary>
    /// Provides a set of options for the archive.
    /// </summary>
    public class ArchiveOptions
    {
        /// <summary>
        /// The storage options of the archive.
        /// </summary>
        public ArchiveStorageOptions StorageOptions { get; } = new ArchiveStorageOptions();

        /// <summary>
        /// The tag options for the archive.
        /// </summary>
        public ArchiveTagOptions TagOptions { get; } = new ArchiveTagOptions();
    }
}
