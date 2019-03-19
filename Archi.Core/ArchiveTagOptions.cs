namespace Archi.Core
{
    /// <summary>
    /// Provides a set of options for tagging an archive.
    /// </summary>
    public class ArchiveTagOptions
    {
        /// <summary>
        /// Provides a set of characters that are allowed to be used on tags associated
        /// with an archive.
        /// </summary>
        public string AllowedCharacters { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    }
}
