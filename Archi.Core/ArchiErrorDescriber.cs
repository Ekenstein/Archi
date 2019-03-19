using System.Globalization;
using Monadicsh;

namespace Archi.Core
{
    /// <summary>
    /// Service to enable localization for application facing archive errors.
    /// </summary>
    public class ArchiErrorDescriber
    {
        /// <summary>
        /// Returns an <see cref="Error"/> indicating that a tag must not be
        /// null or empty when added to an archive.
        /// </summary>
        public virtual Error TagMustNotBeNullOrEmpty => new Error
        {
            Code = nameof(TagMustNotBeNullOrEmpty),
            Description = ArchiResources.TagMustNotBeNullOrEmpty
        };

        /// <summary>
        /// Returns an <see cref="Error"/> indicating that the <paramref name="tag"/>
        /// is already associated with the archive.
        /// </summary>
        /// <param name="tag">The tag that was already associated with the archive.</param>
        /// <returns>
        /// An <see cref="Error"/> describing that the tag was already associated with the archive.
        /// </returns>
        public virtual Error TagAlreadyExist(string tag) => new Error
        {
            Code = nameof(TagAlreadyExist),
            Description = string.Format(CultureInfo.CurrentCulture, ArchiResources.TagAlreadyExist, tag)
        };

        /// <summary>
        /// Returns an <see cref="Error"/> indicating that the <paramref name="tag"/>
        /// is not associated with the archive.
        /// </summary>
        /// <param name="tag">The tag that is not associated with the archive.</param>
        /// <returns>
        /// An <see cref="Error"/> describing that the tag isn't associated with the archive.
        /// </returns>
        public virtual Error TagDoesNotExist(string tag) => new Error
        {
            Code = nameof(TagDoesNotExist),
            Description = string.Format(CultureInfo.CurrentCulture, ArchiResources.TagDoesNotExist, tag)
        };

        /// <summary>
        /// Returns an <see cref="Error"/> indicating that the <paramref name="tag"/>
        /// is invalid.
        /// </summary>
        /// <param name="tag">The invalid tag.</param>
        /// <returns>
        /// An <see cref="Error"/> describing that the <paramref name="tag"/> is invalid.
        /// </returns>
        public virtual Error TagInvalid(string tag) => new Error
        {
            Code = nameof(TagInvalid),
            Description = string.Format(CultureInfo.CurrentCulture, ArchiResources.TagInvalid, tag)
        };
    }
}
