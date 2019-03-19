using System;

namespace Archi.Models
{
    /// <summary>
    /// Represents a <see cref="Models.File"/> that is associated with an <see cref="Models.Archive"/>.
    /// </summary>
    public class ArchiveFile
    {
        /// <summary>
        /// The id of the archive associated with the file.
        /// </summary>
        public Guid ArchiveId { get; set; }

        /// <summary>
        /// The archive associated with the file.
        /// </summary>
        public Archive Archive { get; set; }

        /// <summary>
        /// The id of the file associated with the archive.
        /// </summary>
        public Guid FileId { get; set; }

        /// <summary>
        /// The file associated with the archive.
        /// </summary>
        public File File { get; set; }
    }
}
