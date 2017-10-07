


namespace Cake.AWS.S3
{
    /// <summary>
    /// Represents how to check if a file has been modified
    /// </summary>
    public enum ModifiedCheck
    {
        /// <summary>
        /// By comparing the MD5 hash
        /// </summary>
        Hash = 1,

        /// <summary>
        /// By comparing the modified date
        /// </summary>
        Date = 2
    }
}
