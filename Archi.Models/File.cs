using System;

namespace Archi.Models
{
    public class File
    {
        /// <summary>
        /// The unique ID of the file.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// The content type of the file.
        /// </summary>
        public string ContentType { get; set; }
    }
}
