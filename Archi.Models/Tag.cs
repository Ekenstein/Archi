using System;

namespace Archi.Models
{
    public class Tag
    {
        /// <summary>
        /// The unique ID of the tag.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The name of the tag.
        /// </summary>
        public string Name { get; set; }
    }
}
