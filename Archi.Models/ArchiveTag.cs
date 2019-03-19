using System;

namespace Archi.Models
{
    /// <summary>
    /// Represents a <see cref="Models.Tag"/> that is associated with an <see cref="Models.Archive"/>
    /// </summary>
    public class ArchiveTag
    {
        /// <summary>
        /// The id of the archive associated with the tag.
        /// </summary>
        public Guid ArchiveId { get; set; }

        /// <summary>
        /// The archive associated with the tag.
        /// </summary>
        public Archive Archive { get; set; }

        /// <summary>
        /// The id of the tag associated with the archive.
        /// </summary>
        public Guid TagId { get; set; }

        /// <summary>
        /// The tag associated with the archive.
        /// </summary>
        public Tag Tag { get; set; }
    }
}
