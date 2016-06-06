#region Using Statements
    using System;
    using System.IO;
    using System.Collections.Generic;

    using Cake.Core.IO;

    using Amazon.S3.Model;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// Provides a high level utility for managing transfers to and from Amazon S3.
    /// It makes extensive use of Amazon S3 multipart uploads to achieve enhanced throughput, 
    /// performance, and reliability. When uploading large files by specifying file paths 
    /// instead of a stream, TransferUtility uses multiple threads to upload multiple parts of 
    /// a single upload at once. When dealing with large content sizes and high bandwidth, 
    /// this can increase throughput significantly.
    /// </summary>
    public interface IS3Manager
    {
        #region Functions (11)
            /// <summary>
            /// Syncs the specified directory to Amazon S3, checking the modified date of the local fiels with existing S3Objects.
            /// </summary>
            /// <param name="dirPath">The directory path to sync to S3</param>
            /// <param name="settings">The <see cref="SyncSettings"/> required to sync to Amazon S3.</param>
            /// <returns>A list of keys that require invalidating.</returns>
            IList<string> Sync(DirectoryPath dirPath, SyncSettings settings);



            /// <summary>
            /// Uploads the specified file. For large uploads, the file will be divided and uploaded in parts 
            /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
            /// </summary>
            /// <param name="filePath">The file path of the file to upload.</param>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="settings">The <see cref="UploadSettings"/> required to upload to Amazon S3.</param>
            void Upload(FilePath filePath, string key, UploadSettings settings);

            /// <summary>
            /// Uploads the contents of the specified stream. For large uploads, the file will be divided and uploaded in parts 
            /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
            /// </summary>
            /// <param name="stream">The stream to read to obtain the content to upload.</param>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="settings">The <see cref="UploadSettings"/> required to upload to Amazon S3.</param>
            void Upload(Stream stream, string key, UploadSettings settings);



            /// <summary>
            /// Downloads the content from Amazon S3 and writes it to the specified file.
            /// </summary>
            /// <param name="filePath">The file path of the file to upload.</param>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="version">The identifier for the specific version of the object to be downloaded, if required.</param>
            /// <param name="settings">The <see cref="DownloadSettings"/> required to download from Amazon S3.</param>
            void Download(FilePath filePath, string key, string version, DownloadSettings settings);

            /// <summary>
            /// Opens a stream of the content from Amazon S3
            /// </summary>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="version">The identifier for the specific version of the object to be downloaded, if required.</param>
            /// <param name="settings">The <see cref="DownloadSettings"/> required to download from Amazon S3.</param>
            Stream Open(string key, string version, DownloadSettings settings);

            /// <summary>
            /// Get the byte array of the content from Amazon S3
            /// </summary>
            /// <param name="key">The S3 object key.</param>
            /// <param name="version">The S3 object version.</param>
            /// <param name="settings">The download settings.</param>
            /// <returns>A byte array.</returns>
            byte[] GetBytes(string key, string version, DownloadSettings settings);



            /// <summary>
            /// Removes the null version (if there is one) of an object and inserts a delete
            /// marker, which becomes the latest version of the object. If there isn't a null
            /// version, Amazon S3 does not remove any objects.
            /// </summary>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
            /// <param name="settings">The <see cref="DownloadSettings"/> required to download from Amazon S3.</param>
            void Delete(string key, string version, S3Settings settings);



            /// <summary>
            /// Retrieves object from Amazon S3.
            /// </summary>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
            /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
            S3Object GetObject(string key, string version, S3Settings settings);

            /// <summary>
            /// Returns all the objects in a S3 bucket.
            /// </summary>
            /// <param name="prefix">Limits the response to keys that begin with the specified prefix.</param>
            /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
            IList<S3Object> GetObjects(string prefix, S3Settings settings);



            /// <summary>
            /// Generates a base64-encoded encryption key for Amazon S3 to use to encrypt / decrypt objects
            /// </summary>
            /// <param name="filePath">The file path to store the key in.</param>
            /// <param name="size">The size in bits of the secret key used by the symmetric algorithm</param>
            void GenerateEncryptionKey(FilePath filePath, int size);



            /// <summary>
            /// Create a signed URL allowing access to a resource that would usually require authentication. cts
            /// </summary>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
            /// <param name="expires">The expiry date and time for the pre-signed url. </param>
            /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
            string GetPreSignedURL(string key, string version, DateTime expires, S3Settings settings);
        #endregion
    }
}
