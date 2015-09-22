#region Using Statements
    using System.IO;

    using Cake.Core;
    using Cake.Core.Annotations;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// Amazon S3 aliases
    /// </summary>
    [CakeAliasCategory("AWS.S3")]
    [CakeNamespaceImport("Amazon")]
    [CakeNamespaceImport("Amazon.S3")]
    public static class S3Aliases
    {
        private static ITransferManager CreateManager(this ICakeContext context)
        {
            return new TransferManager(context.Environment, context.Log);
        }



        /// <summary>
        /// Uploads the specified file. For large uploads, the file will be divided and uploaded in parts 
        /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="filePath">The file path of the file to upload.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="settings">The <see cref="UploadSettings"/> required to upload to Amazon S3.</param>
        [CakeMethodAlias]
        public static void S3Upload(this ICakeContext context, string filePath, string key, UploadSettings settings)
        {
            context.CreateManager().Upload(filePath, key, settings);
        }

        /// <summary>
        /// Uploads the contents of the specified stream. For large uploads, the file will be divided and uploaded in parts 
        /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="stream">The stream to read to obtain the content to upload.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="settings">The <see cref="UploadSettings"/> required to upload to Amazon S3.</param>
        [CakeMethodAlias]
        public static void S3Upload(this ICakeContext context, Stream stream, string key, UploadSettings settings)
        {
            context.CreateManager().Upload(stream, key, settings);
        }



        /// <summary>
        /// Downloads the content from Amazon S3 and writes it to the specified file.
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="filePath">The file path of the file to upload.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="settings">The <see cref="DownloadSettings"/> required to download from Amazon S3.</param>
        [CakeMethodAlias]
        public static void S3Download(this ICakeContext context, string filePath, string key, DownloadSettings settings)
        {
            context.CreateManager().Download(filePath, key, settings);
        }
    }
}
