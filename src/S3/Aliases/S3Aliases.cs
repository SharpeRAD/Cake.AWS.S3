#region Using Statements
    using System;
    using System.IO;
    using System.Collections.Generic;

    using Cake.Core;
    using Cake.Core.IO;
    using Cake.Core.Annotations;

    using Amazon.S3.Model;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    ///  Contains Cake aliases for configuring Amazon Simple Storage Service
    /// </summary>
    [CakeAliasCategory("AWS")]
    [CakeNamespaceImport("Amazon")]
    [CakeNamespaceImport("Amazon.S3")]
    public static class S3Aliases
    {
        private static IS3Manager CreateManager(this ICakeContext context)
        {
            return new S3Manager(context.Environment, context.Log);
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
        [CakeAliasCategory("S3")]
        public static void S3Upload(this ICakeContext context, FilePath filePath, string key, UploadSettings settings)
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
        [CakeAliasCategory("S3")]
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
        /// <param name="version">The identifier for the specific version of the object to be downloaded, if required.</param>
        /// <param name="settings">The <see cref="DownloadSettings"/> required to download from Amazon S3.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("S3")]
        public static void S3Download(this ICakeContext context, FilePath filePath, string key, string version, DownloadSettings settings)
        {
            context.CreateManager().Download(filePath, key, version, settings);
        }



        /// <summary>
        /// Removes the null version (if there is one) of an object and inserts a delete
        /// marker, which becomes the latest version of the object. If there isn't a null
        /// version, Amazon S3 does not remove any objects.
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("S3")]
        public static void S3Delete(this ICakeContext context, string key, S3Settings settings)
        {
            context.CreateManager().Delete(key, "", settings);
        }

        /// <summary>
        /// Removes the null version (if there is one) of an object and inserts a delete
        /// marker, which becomes the latest version of the object. If there isn't a null
        /// version, Amazon S3 does not remove any objects.
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("S3")]
        public static void S3Delete(this ICakeContext context, string key, string version, S3Settings settings)
        {
            context.CreateManager().Delete(key, version, settings);
        }



        /// <summary>
        /// Retrieves object from Amazon S3.
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("S3")]
        public static S3Object GetObject(this ICakeContext context, string key, S3Settings settings)
        {
            return context.CreateManager().GetObject(key, "", settings);
        }

        /// <summary>
        /// Retrieves object from Amazon S3.
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("S3")]
        public static S3Object GetObject(this ICakeContext context, string key, string version, S3Settings settings)
        {
            return context.CreateManager().GetObject(key, version, settings);
        }



        /// <summary>
        /// Returns all the objects in a S3 bucket.
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("S3")]
        public static IList<S3Object> GetObjects(this ICakeContext context, S3Settings settings)
        {
            return context.CreateManager().GetObjects(settings);
        }



        /// <summary>
        /// Gets the last modified date of an S3 object
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("S3")]
        public static DateTime S3LastModified(this ICakeContext context, string key, S3Settings settings)
        {
            return context.S3LastModified(key, "", settings);
        }

        /// <summary>
        /// Gets the last modified date of an S3 object
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("S3")]
        public static DateTime S3LastModified(this ICakeContext context, string key, string version, S3Settings settings)
        {
            S3Object result = context.CreateManager().GetObject(key, version, settings);

            if (result != null)
            {
                return result.LastModified;
            }
            else
            {
                return DateTime.MinValue;
            }
        }



        /// <summary>
        /// Generates a base64-encoded encryption key for Amazon S3 to use to encrypt / decrypt objects
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="filePath">The file path to store the key in.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("S3")]
        public static void GenerateEncryptionKey(this ICakeContext context, FilePath filePath)
        {
            context.CreateManager().GenerateEncryptionKey(filePath);
        }



        /// <summary>
        /// Create a signed URL allowing access to a resource that would usually require authentication. cts
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="expires">The expiry date and time for the pre-signed url. </param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("S3")]
        public static string GetPreSignedURL(this ICakeContext context, string key, DateTime expires, S3Settings settings)
        {
            return context.CreateManager().GetPreSignedURL(key, "", expires, settings);
        }

        /// <summary>
        /// Create a signed URL allowing access to a resource that would usually require authentication. cts
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
        /// <param name="expires">The expiry date and time for the pre-signed url. </param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        [CakeMethodAlias]
        [CakeAliasCategory("S3")]
        public static string GetPreSignedURL(this ICakeContext context, string key, string version, DateTime expires, S3Settings settings)
        {
            return context.CreateManager().GetPreSignedURL(key, version, expires, settings);
        }
    }
}
