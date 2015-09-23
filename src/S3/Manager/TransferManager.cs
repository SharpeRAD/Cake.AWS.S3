#region Using Statements
    using System;
    using System.IO;
    using System.Security.Cryptography;

    using Cake.Core;
    using Cake.Core.IO;
    using Cake.Core.Diagnostics;

    using Amazon.S3;
    using Amazon.S3.Model;
    using Amazon.S3.Transfer;
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
    public class TransferManager : ITransferManager
    {
        #region Fields (2)
            private readonly ICakeEnvironment _Environment;
            private readonly ICakeLog _Log;
        #endregion





        #region Constructor (1)
            /// <summary>
            /// Initializes a new instance of the <see cref="TransferManager" /> class.
            /// </summary>
            /// <param name="environment">The environment.</param>
            /// <param name="log">The log.</param>
            public TransferManager(ICakeEnvironment environment, ICakeLog log)
            {
                if (environment == null)
                {
                    throw new ArgumentNullException("environment");
                }
                if (log == null)
                {
                    throw new ArgumentNullException("log");
                }

                _Environment = environment;
                _Log = log;
            }
        #endregion





        #region Functions (6)
            //Helpers
            private TransferUtility GetUtility(S3Settings settings)
            {
                if (settings == null)
                {
                    throw new ArgumentNullException("settings");
                }
                if (String.IsNullOrEmpty(settings.AccessKey))
                {
                    throw new ArgumentNullException("settings.AccessKey");
                }
                if (String.IsNullOrEmpty(settings.SecretKey))
                {
                    throw new ArgumentNullException("settings.SecretKey");
                }

                return new TransferUtility(settings.AccessKey, settings.SecretKey, settings.Region);
            }

            private TransferUtilityUploadRequest CreateUploadRequest(UploadSettings settings)
            {
                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();

                request.BucketName = settings.BucketName;

                request.ServerSideEncryptionCustomerProvidedKey = settings.EncryptionKey;
                request.ServerSideEncryptionCustomerProvidedKeyMD5 = settings.EncryptionKeyMD5;
                request.ServerSideEncryptionCustomerMethod = settings.EncryptionMethod;

                if (!String.IsNullOrEmpty(settings.EncryptionKey))
                {
                    request.ServerSideEncryptionCustomerMethod = ServerSideEncryptionCustomerMethod.AES256;
                }

                request.CannedACL = settings.CannedACL;

                return request;
            }

            private TransferUtilityDownloadRequest CreateDownloadRequest(DownloadSettings settings)
            {
                TransferUtilityDownloadRequest request = new TransferUtilityDownloadRequest();

                request.BucketName = settings.BucketName;

                request.ServerSideEncryptionCustomerProvidedKey = settings.EncryptionKey;
                request.ServerSideEncryptionCustomerProvidedKeyMD5 = settings.EncryptionKeyMD5;
                request.ServerSideEncryptionCustomerMethod = settings.EncryptionMethod;

                if (!String.IsNullOrEmpty(settings.EncryptionKey))
                {
                    request.ServerSideEncryptionCustomerMethod = ServerSideEncryptionCustomerMethod.AES256;
                }

                request.ModifiedSinceDate = settings.ModifiedDate;

                return request;
            }



            private void SetWorkingDirectory(S3Settings settings)
            {
                DirectoryPath workingDirectory = settings.WorkingDirectory ?? _Environment.WorkingDirectory;

                settings.WorkingDirectory = workingDirectory.MakeAbsolute(_Environment);
            }
 
            private void UploadProgressEvent(object sender, UploadProgressArgs e)
            {
                decimal percent = (100 / e.TotalBytes) * e.TransferredBytes;

                _Log.Verbose("{0} ({0}/{1})", percent.ToString("N1") + "%", e.TransferredBytes.ToString("N0"), e.TotalBytes.ToString("N0"));
            }

            private void WriteObjectProgressEvent(object sender, WriteObjectProgressArgs e)
            {
                decimal percent = (100 / e.TotalBytes) * e.TransferredBytes;

                _Log.Verbose("{0} ({0}/{1})", percent.ToString("N1") + "%", e.TransferredBytes.ToString("N0"), e.TotalBytes.ToString("N0"));
            }



            /// <summary>
            /// Uploads the specified file. For large uploads, the file will be divided and uploaded in parts 
            /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
            /// </summary>
            /// <param name="filePath">The file path of the file to upload.</param>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="settings">The <see cref="UploadSettings"/> required to upload to Amazon S3.</param>
            public void Upload(FilePath filePath, string key, UploadSettings settings)
            {
                TransferUtility utility = this.GetUtility(settings);
                TransferUtilityUploadRequest request = this.CreateUploadRequest(settings);

                this.SetWorkingDirectory(settings);
                string fullPath = filePath.MakeAbsolute(settings.WorkingDirectory).FullPath;

                request.FilePath = fullPath;
                request.Key = key;

                request.UploadProgressEvent += new EventHandler<UploadProgressArgs>(UploadProgressEvent);

                _Log.Verbose("Uploading file {0} to bucket {1}...", key, settings.BucketName);
                utility.Upload(request);
            }

            /// <summary>
            /// Uploads the contents of the specified stream. For large uploads, the file will be divided and uploaded in parts 
            /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
            /// </summary>
            /// <param name="stream">The stream to read to obtain the content to upload.</param>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="settings">The <see cref="UploadSettings"/> required to upload to Amazon S3.</param>
            public void Upload(Stream stream, string key, UploadSettings settings)
            {
                TransferUtility utility = this.GetUtility(settings);
                TransferUtilityUploadRequest request = this.CreateUploadRequest(settings);

                request.InputStream = stream;
                request.Key = key;

                request.UploadProgressEvent += new EventHandler<UploadProgressArgs>(UploadProgressEvent);

                _Log.Verbose("Uploading file {0} to bucket {1}...", key, settings.BucketName);
                utility.Upload(request);
            }



            /// <summary>
            /// Downloads the content from Amazon S3 and writes it to the specified file.
            /// </summary>
            /// <param name="filePath">The file path of the file to upload.</param>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="settings">The <see cref="DownloadSettings"/> required to download from Amazon S3.</param>
            public void Download(FilePath filePath, string key, DownloadSettings settings)
            {
                TransferUtility utility = this.GetUtility(settings);
                TransferUtilityDownloadRequest request = this.CreateDownloadRequest(settings);

                this.SetWorkingDirectory(settings);
                string fullPath = filePath.MakeAbsolute(settings.WorkingDirectory).FullPath;

                request.FilePath = fullPath;
                request.Key = key;

                request.WriteObjectProgressEvent += new EventHandler<WriteObjectProgressArgs>(this.WriteObjectProgressEvent);

                _Log.Verbose("Downloading file {0} from bucket {1}...", key, settings.BucketName);
                utility.Download(request);
            }



            /// <summary>
            /// Generates a base64-encoded encryption key for Amazon S3 to use to encrypt / decrypt objects
            /// </summary>
            /// <param name="filePath">The file path to store the key in.</param>
            public void GenenrateEncryptionKey(FilePath filePath)
            {
                string fullPath = filePath.MakeAbsolute(_Environment.WorkingDirectory).FullPath;

                Aes aesEncryption = Aes.Create();
                aesEncryption.KeySize = 256;
                aesEncryption.GenerateKey();

                string base64Key = Convert.ToBase64String(aesEncryption.Key);
                File.WriteAllText(fullPath, base64Key);
            }
        #endregion
    }
}
