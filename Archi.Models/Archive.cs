using System;
using System.Collections.Generic;

namespace Archi.Models
{
    /// <summary>
    /// Represents an archive.
    /// </summary>
    public class Archive
    {
        /// <summary>
        /// The unique identifier of this archive.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The description describing the archive.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The date and time in UTC the archive was created.
        /// </summary>
        public DateTime Created { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The files associated with this archive.
        /// </summary>
        public ICollection<ArchiveFile> Files { get; } = new List<ArchiveFile>();

        /// <summary>
        /// The tags associated with this archive.
        /// </summary>
        public ICollection<ArchiveTag> Tags { get; } = new List<ArchiveTag>();
    }
}
