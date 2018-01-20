#region Using Statements
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

using Cake.Core;
using Cake.Core.IO;
using Cake.Core.Diagnostics;

using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

using HeyRed.Mime;
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
    public class S3Manager : IS3Manager
    {
        #region Fields
        private readonly IFileSystem _FileSystem;
        private readonly ICakeEnvironment _Environment;
        private readonly ICakeLog _Log;
        #endregion





        #region Constructor (1)
        /// <summary>
        /// Initializes a new instance of the <see cref="S3Manager" /> class.
        /// </summary>
        /// <param name="fileSystem">The file System.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="log">The log.</param>
        public S3Manager(IFileSystem fileSystem, ICakeEnvironment environment, ICakeLog log)
        {
            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _FileSystem = fileSystem;
            _Environment = environment;
            _Log = log;

            this.LogProgress = true;
        }
        #endregion





        #region Properties
        /// <summary>
        /// If the manager should output progrtess events to the cake log
        /// </summary>
        public bool LogProgress { get; set; }
        #endregion





        #region Helper Methods
        //Request
        private AmazonS3Client GetClient(S3Settings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
                
            if (settings.Region == null)
            {
                throw new ArgumentNullException("settings.Region");
            }

            if (settings.Credentials == null)
            {
                if (String.IsNullOrEmpty(settings.AccessKey))
                {
                    throw new ArgumentNullException("settings.AccessKey");
                }
                if (String.IsNullOrEmpty(settings.SecretKey))
                {
                    throw new ArgumentNullException("settings.SecretKey");
                }

                if(!String.IsNullOrEmpty(settings.SessionToken))
                {
                    return new AmazonS3Client(settings.AccessKey, settings.SecretKey, settings.SessionToken, settings.Region);
                }

                return new AmazonS3Client(settings.AccessKey, settings.SecretKey, settings.Region);
            }
            else
            {
                return new AmazonS3Client(settings.Credentials, settings.Region);
            }
        }

        private TransferUtility GetUtility(S3Settings settings)
        {
            return new TransferUtility(this.GetClient(settings));
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

            if (settings.Headers != null)
            {
                foreach (string key in settings.Headers.Keys)
                {
                    request.Headers[key] = settings.Headers[key];
                }
            }

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
        
        private TransferUtilityOpenStreamRequest CreateOpenRequest(DownloadSettings settings)
        {
            TransferUtilityOpenStreamRequest request = new TransferUtilityOpenStreamRequest();

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
        
        private PutObjectRequest CreatePutObjectRequest(UploadSettings settings)
        {
            PutObjectRequest request = new PutObjectRequest();

            request.BucketName = settings.BucketName;

            request.ServerSideEncryptionCustomerProvidedKey = settings.EncryptionKey;
            request.ServerSideEncryptionCustomerProvidedKeyMD5 = settings.EncryptionKeyMD5;
            request.ServerSideEncryptionCustomerMethod = settings.EncryptionMethod;

            if (!String.IsNullOrEmpty(settings.EncryptionKey))
            {
                request.ServerSideEncryptionCustomerMethod = ServerSideEncryptionCustomerMethod.AES256;
            }

            request.CannedACL = settings.CannedACL;

            if (settings.Headers != null)
            {
                foreach (string key in settings.Headers.Keys)
                {
                    request.Headers[key] = settings.Headers[key];
                }
            }

            return request;
        }

        private GetObjectRequest CreateGetObjectRequest(string key, string version, S3Settings settings)
        {
            GetObjectRequest request = new GetObjectRequest();

            request.BucketName = settings.BucketName;
            request.Key = key;

            if (!String.IsNullOrEmpty(version))
            {
                request.VersionId = version;
            }

            request.ServerSideEncryptionCustomerProvidedKey = settings.EncryptionKey;
            request.ServerSideEncryptionCustomerProvidedKeyMD5 = settings.EncryptionKeyMD5;
            request.ServerSideEncryptionCustomerMethod = settings.EncryptionMethod;

            if (!String.IsNullOrEmpty(settings.EncryptionKey))
            {
                request.ServerSideEncryptionCustomerMethod = ServerSideEncryptionCustomerMethod.AES256;
            }

            return request;
        }
        
        private GetObjectMetadataRequest CreateGetObjectMetadataRequest(string key, string version, S3Settings settings)
        {
            GetObjectMetadataRequest request = new GetObjectMetadataRequest();

            request.BucketName = settings.BucketName;
            request.Key = key;

            if (!String.IsNullOrEmpty(version))
            {
                request.VersionId = version;
            }

            request.ServerSideEncryptionCustomerProvidedKey = settings.EncryptionKey;
            request.ServerSideEncryptionCustomerProvidedKeyMD5 = settings.EncryptionKeyMD5;
            request.ServerSideEncryptionCustomerMethod = settings.EncryptionMethod;

            if (!String.IsNullOrEmpty(settings.EncryptionKey))
            {
                request.ServerSideEncryptionCustomerMethod = ServerSideEncryptionCustomerMethod.AES256;
            }

            return request;
        }



        //Directory
        private void SetWorkingDirectory(S3Settings settings)
        {
            DirectoryPath workingDirectory = settings.WorkingDirectory ?? _Environment.WorkingDirectory;

            settings.WorkingDirectory = workingDirectory.MakeAbsolute(_Environment);
        }
 


        //Progress
        private decimal GetPercent(TransferProgressArgs e)
        {
            return (((decimal)1 / (decimal)e.TotalBytes) * (decimal)e.TransferredBytes) * 100;
        }

        private void UploadProgressEvent(object sender, UploadProgressArgs e)
        {
            if (this.LogProgress)
            {
                _Log.Verbose("{0} ({1}/{2})", this.GetPercent(e).ToString("N2") + "%", (e.TransferredBytes / 1000).ToString("N0"), (e.TotalBytes / 1000).ToString("N0"));
            }
        }

        private void WriteObjectProgressEvent(object sender, WriteObjectProgressArgs e)
        {
            if (this.LogProgress)
            {
                _Log.Verbose("{0} ({1}/{2})", this.GetPercent(e).ToString("N2") + "%", (e.TransferredBytes / 1000).ToString("N0"), (e.TotalBytes / 1000).ToString("N0"));
            }
        }



        /// <summary>
        /// Gets the hash of a file
        /// </summary>
        /// <param name="file">The file to calculate the hash of</param>
        /// <returns>The hash of a file.</returns>
        public string GetHash(IFile file)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = file.OpenRead())
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-","").ToLower();
                }
            }
        }

        private string GetKey(IFile file, string fullPath, bool lowerPaths, string prefix)
        {
            string key;

            //Replace Path
            if (lowerPaths)
            {
                key = file.Path.FullPath.ToLower().Replace(fullPath.ToLower(), "");
            }
            else
            {
                key = file.Path.FullPath.Replace(fullPath, "");
            }
        
            //Correct folders
            key = key.Replace("//", "/");
            if (key.StartsWith("./"))
            {
                key = key.Substring(2, key.Length - 2);
            }

            //Add prefix
            if (!String.IsNullOrEmpty(prefix))
            {
                if (lowerPaths)
                {
                    prefix = prefix.ToLower();
                }

                if (!key.StartsWith(prefix))
                {
                    key = prefix + key;
                }
            }

            return key;
        }

        private string GetPath(string fullPath, string key)
        {
            fullPath = fullPath.Replace("//", "/");

            if (!fullPath.EndsWith("/"))
            {
                fullPath += "/";
            }

            return fullPath + key;
        }
        #endregion





        #region Main Methods
        /// <summary>
        /// Syncs the specified directory to Amazon S3, checking the modified date of the local files with existing S3Objects and uploading them if its changes.
        /// </summary>
        /// <param name="dirPath">The directory path to sync to S3</param>
        /// <param name="settings">The <see cref="SyncSettings"/> required to sync to Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A list of keys that require invalidating.</returns>
        public async Task<IList<string>> SyncUpload(DirectoryPath dirPath, SyncSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Get Directory
            this.SetWorkingDirectory(settings);
            string fullPath = dirPath.MakeAbsolute(settings.WorkingDirectory).FullPath;
            if (!fullPath.EndsWith("/"))
            {
                fullPath += "/";
            }

            IDirectory dir = _FileSystem.GetDirectory(dirPath.MakeAbsolute(settings.WorkingDirectory));
            List<string> list = new List<string>();

            if (settings.ModifiedCheck == ModifiedCheck.Hash)
            {
                settings.GenerateETag = true;
            }



            if (dir.Exists)
            {
                //Get S3 Objects
                IList<S3Object> objects = await this.GetObjects(settings.KeyPrefix, settings, cancellationToken);



                //Check Files
                IEnumerable<IFile> files = dir.GetFiles(settings.SearchFilter, settings.SearchScope);
                IList<SyncPath> upload = new List<SyncPath>();

                foreach (IFile file in files)
                {
                    //Get Key
                    string key = this.GetKey(file, fullPath, settings.LowerPaths, settings.KeyPrefix);



                    //Get ETag
                    string eTag = "";
                    if (settings.GenerateETag || (settings.ModifiedCheck == ModifiedCheck.Hash))
                    {
                        eTag = this.GetHash(file);
                    }



                    //Check Modified
                    S3Object obj = objects.FirstOrDefault(o => o.Key == key);

                    if ((obj == null) 
                        || ((settings.ModifiedCheck == ModifiedCheck.Hash) && (obj.ETag != "\"" + eTag + "\""))
                        || ((settings.ModifiedCheck == ModifiedCheck.Date) && ((DateTimeOffset)new FileInfo(file.Path.FullPath).LastWriteTime) > (DateTimeOffset)obj.LastModified) )
                    {
                        upload.Add(new SyncPath()
                        {
                            Path = file.Path,
                            Key = key,
                            ETag = eTag
                        });

                        list.Add(key);
                    }

                    if (obj != null)
                    {
                        objects.Remove(obj);
                    }
                }



                //Upload
                this.LogProgress = false;
                await this.Upload(upload, settings, cancellationToken);



                //Delete
                list.AddRange(objects.Select(o => o.Key).ToList());

                await this.Delete(objects.Select(o => o.Key).ToList(), settings, cancellationToken);
            }

            return list;
        }

        /// <summary>
        /// Syncs the specified directory from Amazon S3, checking the modified date of the local files with existing S3Objects and downloading them if its changed.
        /// </summary>
        /// <param name="dirPath">The directory path to sync to S3</param>
        /// <param name="settings">The <see cref="SyncSettings"/> required to sync to Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A list of keys that require invalidating.</returns>
        public async Task<IList<string>> SyncDownload(DirectoryPath dirPath, SyncSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Get Directory
            this.SetWorkingDirectory(settings);
            string fullPath = dirPath.MakeAbsolute(settings.WorkingDirectory).FullPath;
            if (!fullPath.EndsWith("/"))
            {
                fullPath += "/";
            }

            IDirectory dir = _FileSystem.GetDirectory(dirPath.MakeAbsolute(settings.WorkingDirectory));
            List<string> list = new List<string>();

            if (settings.ModifiedCheck == ModifiedCheck.Hash)
            {
                settings.GenerateETag = true;
            }



            if (dir.Exists)
            {
                //Get S3 Objects
                IList<S3Object> objects = await this.GetObjects(settings.KeyPrefix, settings, cancellationToken);
                IEnumerable<IFile> files = dir.GetFiles(settings.SearchFilter, settings.SearchScope);

                IList<SyncPath> download = new List<SyncPath>();

                List<IFile> delete = new List<IFile>();
                delete.AddRange(files);



                foreach (S3Object obj in objects)
                {
                    //Find File
                    IFile file = files.FirstOrDefault(f => 
                    {
                        return (this.GetKey(f, fullPath, settings.LowerPaths, settings.KeyPrefix) == obj.Key);
                    });

                    if (file != null)
                    {
                        //Get Key
                        string key = this.GetKey(file, fullPath, settings.LowerPaths, settings.KeyPrefix);



                        //Get ETag
                        string eTag = "";
                        if (settings.GenerateETag || (settings.ModifiedCheck == ModifiedCheck.Hash))
                        {
                            eTag = this.GetHash(file);
                        }



                        //Check Modified
                        if (((settings.ModifiedCheck == ModifiedCheck.Hash) && (obj.ETag != "\"" + eTag + "\""))
                            || ((settings.ModifiedCheck == ModifiedCheck.Date) && ((DateTimeOffset)new FileInfo(file.Path.FullPath).LastWriteTime) < (DateTimeOffset)obj.LastModified))
                        {
                            download.Add(new SyncPath()
                            {
                                Path = file.Path,
                                Key = key
                            });

                            list.Add(key);
                        }

                        delete.Remove(file);
                    }
                    else
                    {
                        download.Add(new SyncPath()
                        {
                            Path = this.GetPath(fullPath, obj.Key),
                            Key = obj.Key
                        });

                        list.Add(obj.Key);
                    }
                }



                //Download
                this.LogProgress = false;
                await this.Download(download, settings, cancellationToken);



                //Delete
                foreach (IFile file in delete)
                {
                    file.Delete();
                    
                    _Log.Verbose("Deleting file {0}", file.Path.FullPath);
                }
            }

            return list;
        }



        /// <summary>
        /// Syncs the specified file to Amazon S3, checking the modified date of the local files with existing S3Objects and uploading them if its changes.
        /// </summary>
        /// <param name="filePath">The file path to sync to S3</param>
        /// <param name="settings">The <see cref="SyncSettings"/> required to sync to Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The key that require invalidating.</returns>
        public async Task<string> SyncUpload(FilePath filePath, SyncSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Get Directory
            this.SetWorkingDirectory(settings);
            string fullPath = filePath.MakeAbsolute(settings.WorkingDirectory).FullPath;

            IFile file = _FileSystem.GetFile(filePath.MakeAbsolute(settings.WorkingDirectory));

            if (settings.ModifiedCheck == ModifiedCheck.Hash)
            {
                settings.GenerateETag = true;
            }

            fullPath = fullPath.Replace(file.Path.GetFilename().FullPath, "");

            string key = this.GetKey(file, fullPath, settings.LowerPaths, settings.KeyPrefix);
            S3Object obj = await this.GetObject(key, "", settings, cancellationToken);



            if (file.Exists)
            {
                //Get ETag
                string eTag = "";
                if (settings.GenerateETag || (settings.ModifiedCheck == ModifiedCheck.Hash))
                {
                    eTag = this.GetHash(file);
                }



                //Check Modified
                IList<SyncPath> upload = new List<SyncPath>();

                if ((obj == null)
                    || ((settings.ModifiedCheck == ModifiedCheck.Hash) && (obj.ETag != "\"" + eTag + "\""))
                    || ((settings.ModifiedCheck == ModifiedCheck.Date) && ((DateTimeOffset)new FileInfo(file.Path.FullPath).LastWriteTime) > (DateTimeOffset)obj.LastModified))
                {
                    upload.Add(new SyncPath()
                    {
                        Path = file.Path,
                        Key = key,
                        ETag = eTag
                    });
                }



                //Upload
                this.LogProgress = false;
                await this.Upload(upload, settings, cancellationToken);

                return key;
            }
            else if (obj != null)
            {
                await this.Delete(key, "", settings, cancellationToken);

                return key;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Syncs the specified file from Amazon S3, checking the modified date of the local files with existing S3Objects and downloading them if its changed.
        /// </summary>
        /// <param name="filePath">The file path to sync to S3</param>
        /// <param name="settings">The <see cref="SyncSettings"/> required to sync to Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The key that require invalidating.</returns>
        public async Task<string> SyncDownload(FilePath filePath, SyncSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Get Directory
            this.SetWorkingDirectory(settings);
            string fullPath = filePath.MakeAbsolute(settings.WorkingDirectory).FullPath;
            if (!fullPath.EndsWith("/"))
            {
                fullPath += "/";
            }

            IFile file = _FileSystem.GetFile(filePath.MakeAbsolute(settings.WorkingDirectory));

            if (settings.ModifiedCheck == ModifiedCheck.Hash)
            {
                settings.GenerateETag = true;
            }
                
            fullPath = fullPath.Replace(file.Path.GetFilename().FullPath, "");

            string key = this.GetKey(file, fullPath, settings.LowerPaths, settings.KeyPrefix);
            S3Object obj = await this.GetObject(key, "", settings, cancellationToken);

            IList<SyncPath> download = new List<SyncPath>();



            if ((file.Exists) && (obj != null))
            {
                //Get ETag
                string eTag = "";
                if (settings.GenerateETag || (settings.ModifiedCheck == ModifiedCheck.Hash))
                {
                    eTag = this.GetHash(file);
                }



                //Check Modified
                if (((settings.ModifiedCheck == ModifiedCheck.Hash) && (obj.ETag != "\"" + eTag + "\""))
                    || ((settings.ModifiedCheck == ModifiedCheck.Date) && ((DateTimeOffset)new FileInfo(file.Path.FullPath).LastWriteTime) < (DateTimeOffset)obj.LastModified))
                {
                    download.Add(new SyncPath()
                    {
                        Path = file.Path,
                        Key = key
                    });
                }
            }
            else if (obj != null)
            {
                download.Add(new SyncPath()
                {
                    Path = this.GetPath(fullPath, obj.Key),
                    Key = obj.Key
                });
            }
            else
            {
                //Delete
                file.Delete();
                    
                _Log.Verbose("Deleting file {0}", file.Path.FullPath);
                return key;
            }



            //Download
            this.LogProgress = false;
            await this.Download(download, settings, cancellationToken);

            return (download.Count > 0) ? key : "";
        }



        /// <summary>
        /// Uploads a collection of files to S3. For large uploads, the file will be divided and uploaded in parts 
        /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
        /// </summary>
        /// <param name="paths">The paths to upload.</param>
        /// <param name="settings">The <see cref="SyncSettings"/> required to upload to Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task Upload(IList<SyncPath> paths, SyncSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach(SyncPath path in paths)
            {
                try
                {
                    UploadSettings copied = new UploadSettings()
                    {
                        WorkingDirectory = settings.WorkingDirectory,

                        AccessKey = settings.AccessKey,
                        SecretKey = settings.SecretKey,
                        SessionToken = settings.SessionToken,
                        Credentials = settings.Credentials,

                        Region = settings.Region,
                        BucketName = settings.BucketName,

                        EncryptionMethod = settings.EncryptionMethod,
                        EncryptionKey = settings.EncryptionKey,
                        EncryptionKeyMD5 = settings.EncryptionKeyMD5,

                        CannedACL = settings.CannedACL,
                        StorageClass = settings.StorageClass,
                        KeyManagementServiceKeyId = settings.KeyManagementServiceKeyId,

                        Headers = new HeadersCollection(),

                        GenerateContentType = settings.GenerateContentType,
                        DefaultContentType = settings.DefaultContentType,
                        GenerateContentLength = settings.GenerateContentLength,
                        GenerateETag = settings.GenerateETag,
                        GenerateHashTag = settings.GenerateHashTag,

                        CompressContent = settings.CompressContent,
                        CompressExtensions = settings.CompressExtensions
                    };



                    if (!String.IsNullOrEmpty(path.ETag))
                    {
                        copied.Headers["ETag"] = path.ETag;
                    }

                    foreach (string header in settings.Headers.Keys)
                    {
                        copied.Headers[header] = settings.Headers[header];
                    }

                    await this.Upload(path.Path, path.Key, copied, cancellationToken);
                }
                catch (Exception ex)
                {
                    _Log.Error(ex.Message);
                }
            }
        }

        /// <summary>
        /// Uploads the specified file. For large uploads, the file will be divided and uploaded in parts 
        /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
        /// </summary>
        /// <param name="filePath">The file path of the file to upload.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="settings">The <see cref="UploadSettings"/> required to upload to Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task Upload(FilePath filePath, string key, UploadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (settings.CompressContent && settings.CompressExtensions.Contains(filePath.GetExtension()))
            {
                settings.GenerateContentLength = true;

                await this.UploadCompressed(filePath, key, settings, cancellationToken);
            }
            else
            {
                await this.UploadUnCompressed(filePath, key, settings, cancellationToken);
            }
        }

        /// <summary>
        /// Uploads the specified file. For large uploads, the file will be divided and uploaded in parts 
        /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
        /// </summary>
        /// <param name="filePath">The file path of the file to upload.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="settings">The <see cref="UploadSettings"/> required to upload to Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task UploadUnCompressed(FilePath filePath, string key, UploadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            TransferUtility utility = this.GetUtility(settings);
            TransferUtilityUploadRequest request = this.CreateUploadRequest(settings);

            this.SetWorkingDirectory(settings);
            string fullPath = filePath.MakeAbsolute(settings.WorkingDirectory).FullPath;

            request.FilePath = fullPath;
            request.Key = key;



            //Set ContentType
            if (settings.GenerateContentType && String.IsNullOrEmpty(request.Headers.ContentType))
            {
                request.Headers.ContentType = GetContentType(filePath, settings);
            }



            //Get File
            IFile file = null;

            if (settings.GenerateETag || settings.GenerateHashTag || settings.GenerateContentLength)
            {
                file = _FileSystem.GetFile(fullPath);
            }



            //Set Hash Tag
            string hash = "";

            if (!String.IsNullOrEmpty(request.Headers["ETag"]))
            {
                hash = request.Headers["ETag"];
            }
            else if ((settings.GenerateETag || settings.GenerateHashTag) && (file != null))
            {
                hash = this.GetHash(file);
                request.Headers["ETag"] = hash;
            }

            if (settings.GenerateHashTag && !String.IsNullOrEmpty(hash))
            {
                request.Metadata.Add("HashTag", hash);
            }

            if (settings.GenerateContentLength && (file != null))
            {
                request.Headers.ContentLength = file.Length;
            }

            if (!String.IsNullOrEmpty(settings.CacheControl))
            {
                request.Headers.CacheControl = settings.CacheControl;
            }



            // Upload
            request.UploadProgressEvent += new EventHandler<UploadProgressArgs>(UploadProgressEvent);

            _Log.Verbose("Uploading file {0} to bucket {1}...", key, settings.BucketName);
            await utility.UploadAsync(request, cancellationToken);
        }

        /// <summary>
        /// Uploads the specified file. For large uploads, the file will be divided and uploaded in parts 
        /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
        /// </summary>
        /// <param name="filePath">The file path of the file to upload.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="settings">The <see cref="UploadSettings"/> required to upload to Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task UploadCompressed(FilePath filePath, string key, UploadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            AmazonS3Client client = this.GetClient(settings);
            PutObjectRequest request = this.CreatePutObjectRequest(settings);

            this.SetWorkingDirectory(settings);
            string fullPath = filePath.MakeAbsolute(settings.WorkingDirectory).FullPath;

            request.Key = key;



            //Set ContentType
            if (settings.GenerateContentType && String.IsNullOrEmpty(request.Headers.ContentType))
            {
                request.Headers.ContentType = GetContentType(filePath, settings);
            }



            //Get File
            IFile file = _FileSystem.GetFile(fullPath);



            //Set Hash Tag
            string hash = "";

            if (!String.IsNullOrEmpty(request.Headers["ETag"]))
            {
                hash = request.Headers["ETag"];
            }
            else if ((settings.GenerateETag || settings.GenerateHashTag) && (file != null))
            {
                hash = this.GetHash(file);
                request.Headers["ETag"] = hash;
            }

            if (settings.GenerateHashTag && !String.IsNullOrEmpty(hash))
            {
                request.Metadata.Add("HashTag", hash);
            }

                

            // Content
            request.InputStream = this.CompressStream(file);

            request.Headers.ContentLength = request.InputStream.Length;
            request.Headers.ContentEncoding = "gzip";

            if (!String.IsNullOrEmpty(settings.CacheControl))
            {
                request.Headers.CacheControl = settings.CacheControl;
            }



            // Upload
            _Log.Verbose("Uploading file {0} to bucket {1}...", key, settings.BucketName);
            await client.PutObjectAsync(request, cancellationToken);
        }

        private static string GetContentType(FilePath filePath, UploadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Default
            const string defaultMimeType = "application/octet-stream";

            if (!filePath.HasExtension)
            {
                if (!String.IsNullOrEmpty(settings.DefaultContentType))
                {
                    return settings.DefaultContentType;
                }
                else
                {
                    return defaultMimeType;
                }
            }

            // From extension
            var contentType = MimeTypesMap.GetMimeType(filePath.GetFilename().FullPath);

            if (!String.IsNullOrEmpty(settings.DefaultContentType) && (contentType == defaultMimeType))
            {
                contentType = settings.DefaultContentType;
            }

            return contentType;
        }

        private Stream CompressStream(IFile file)
        {
            var compressed = new MemoryStream();

            using (Stream decompressed = file.OpenRead())
            {
                using (var zip = new GZipStream(compressed, CompressionLevel.Optimal, true))
                {
                    decompressed.CopyTo(zip);
                }
            }

            compressed.Seek(0, SeekOrigin.Begin);
            return compressed;
        }



        /// <summary>
        /// Uploads the contents of the specified stream. For large uploads, the file will be divided and uploaded in parts 
        /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
        /// </summary>
        /// <param name="stream">The stream to read to obtain the content to upload.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="settings">The <see cref="UploadSettings"/> required to upload to Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task Upload(Stream stream, string key, UploadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            TransferUtility utility = this.GetUtility(settings);
            TransferUtilityUploadRequest request = this.CreateUploadRequest(settings);

            request.InputStream = stream;
            request.Key = key;

            request.UploadProgressEvent += new EventHandler<UploadProgressArgs>(UploadProgressEvent);

            _Log.Verbose("Uploading file {0} to bucket {1}...", key, settings.BucketName);
            await utility.UploadAsync(request, cancellationToken);
        }



        /// <summary>
        /// Downloads a collection of files from S3 and writes ithem to the specified files.
        /// </summary>
        /// <param name="paths">The paths to upload.</param>
        ///  <param name="settings">The <see cref="SyncSettings"/> required to download from Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task Download(IList<SyncPath> paths, SyncSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach(SyncPath path in paths)
            {
                try
                {
                    DownloadSettings copied = new DownloadSettings()
                    {
                        WorkingDirectory = settings.WorkingDirectory,

                        AccessKey = settings.AccessKey,
                        SecretKey = settings.SecretKey,
                        SessionToken = settings.SessionToken,
                        Credentials = settings.Credentials,

                        Region = settings.Region,
                        BucketName = settings.BucketName,

                        EncryptionMethod = settings.EncryptionMethod,
                        EncryptionKey = settings.EncryptionKey,
                        EncryptionKeyMD5 = settings.EncryptionKeyMD5
                    };

                    await this.Download(path.Path, path.Key, "", copied, cancellationToken);
                }
                catch (Exception ex)
                {
                    _Log.Error(ex.Message);
                }
            }
        }

        /// <summary>
        /// Downloads the content from Amazon S3 and writes it to the specified file.
        /// </summary>
        /// <param name="filePath">The file path of the file to upload.</param>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="version">The identifier for the specific version of the object to be downloaded, if required.</param>
        /// <param name="settings">The <see cref="DownloadSettings"/> required to download from Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task Download(FilePath filePath, string key, string version, DownloadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            TransferUtility utility = this.GetUtility(settings);
            TransferUtilityDownloadRequest request = this.CreateDownloadRequest(settings);

            this.SetWorkingDirectory(settings);
            string fullPath = filePath.MakeAbsolute(settings.WorkingDirectory).FullPath;

            request.FilePath = fullPath;
            request.Key = key;

            if (!String.IsNullOrEmpty(version))
            {
                request.VersionId = version;
            }

            request.WriteObjectProgressEvent += new EventHandler<WriteObjectProgressArgs>(this.WriteObjectProgressEvent);

            _Log.Verbose("Downloading file {0} from bucket {1}...", key, settings.BucketName);
            await utility.DownloadAsync(request, cancellationToken);
        }

        /// <summary>
        /// Opens a stream of the content from Amazon S3
        /// </summary>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="version">The identifier for the specific version of the object to be downloaded, if required.</param>
        /// <param name="settings">The <see cref="DownloadSettings"/> required to download from Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A stream.</returns>
        public async Task<Stream> Open(string key, string version, DownloadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            TransferUtility utility = this.GetUtility(settings);
            TransferUtilityOpenStreamRequest request = this.CreateOpenRequest(settings);

            request.Key = key;
            if (!String.IsNullOrEmpty(version))
            {
                request.VersionId = version;
            }

            _Log.Verbose("Opening stream {0} from bucket {1}...", key, settings.BucketName);
            return await utility.OpenStreamAsync(request, cancellationToken);
        }

        /// <summary>
        /// Get the byte array of the content from Amazon S3
        /// </summary>
        /// <param name="key">The S3 object key.</param>
        /// <param name="version">The S3 object version.</param>
        /// <param name="settings">The download settings.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>A byte array.</returns>
        public async Task<byte[]> GetBytes(string key, string version, DownloadSettings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            byte[] data;

            using (Stream input = await this.Open(key, version, settings, cancellationToken))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    input.CopyTo(ms);
                    data = ms.ToArray();
                }
            }

            return data;
        }



        /// <summary>
        /// Deletes a collection of files to S3. For large uploads, the file will be divided and uploaded in parts 
        /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
        /// </summary>
        /// <param name="keys">The set of keys under which the Amazon S3 object is stored.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to upload to Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task Delete(IList<string> keys, S3Settings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach(string key in keys)
            {
                await this.Delete(key, "", settings, cancellationToken);
            }
        }

        /// <summary>
        /// Removes the null version (if there is one) of an object and inserts a delete
        /// marker, which becomes the latest version of the object. If there isn't a null
        /// version, Amazon S3 does not remove any objects.
        /// </summary>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task Delete(string key, string version, S3Settings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            AmazonS3Client client = this.GetClient(settings);
            DeleteObjectRequest request = new DeleteObjectRequest();

            request.BucketName = settings.BucketName;
            request.Key = key;

            if (!String.IsNullOrEmpty(version))
            {
                request.VersionId = version;
            }

            _Log.Verbose("Deleting object {0} from bucket {1}...", key, settings.BucketName);
            await client.DeleteObjectAsync(request, cancellationToken);
        }

        /// <summary>
        /// Removes all objects from the bucket
        /// </summary>
        /// <param name="prefix">Only delete objects that begin with the specified prefix.</param>
        /// <param name="lastModified">Only delete objects that where modified prior to this date.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to delete from Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<IList<string>> DeleteAll(string prefix, DateTimeOffset lastModified, S3Settings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Get S3 Objects
            IList<S3Object> objects = await this.GetObjects(prefix, settings, cancellationToken);
            List<string> list = new List<string>();

            foreach (S3Object obj in objects)
            {
                if ((lastModified == DateTimeOffset.MinValue) || (obj.LastModified < lastModified))
                {
                    list.Add(obj.Key);
                }
            }



            //Delete
            AmazonS3Client client = this.GetClient(settings);

            while (list.Count > 0)
            {
                int max = list.Count;
                if (max > 1000)
                {
                    max = 1000;
                }

                DeleteObjectsRequest request = new DeleteObjectsRequest();
                request.BucketName = settings.BucketName;
                
                for (int index = 0; index < max; index++)
                {
                    request.AddKey(list[index]);
                    _Log.Verbose("Deleting object {0} from bucket {1}...", list[index], settings.BucketName);
                }

                await client.DeleteObjectsAsync(request, cancellationToken);

                list.RemoveRange(0, max);
            }
                
            return list;
        }



        /// <summary>
        /// Retrieves object from Amazon S3.
        /// </summary>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<S3Object> GetObject(string key, string version, S3Settings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            AmazonS3Client client = this.GetClient(settings);
            GetObjectRequest request = this.CreateGetObjectRequest(key, version, settings);

            _Log.Verbose("Get object {0} from bucket {1}...", key, settings.BucketName);

            try
            {
                GetObjectResponse response = await client.GetObjectAsync(request, cancellationToken);

                return new S3Object()
                {
                    Key = response.Key,
                    ETag = response.ETag,

                    LastModified = response.LastModified,
                    StorageClass = response.StorageClass
                };
            }
            catch
            {
                _Log.Verbose("The object {0} does not exist in bucket {1}...", key, settings.BucketName);
                return null;
            }
        }

        /// <summary>
        /// Retrieves object Metadata from Amazon S3.
        /// </summary>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<MetadataCollection> GetObjectMetaData(string key, string version, S3Settings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            AmazonS3Client client = this.GetClient(settings);
            GetObjectMetadataRequest request = this.CreateGetObjectMetadataRequest(key, version, settings);

            _Log.Verbose("Get object {0} from bucket {1}...", key, settings.BucketName);

            try
            {
                GetObjectMetadataResponse response = await client.GetObjectMetadataAsync(request, cancellationToken);

                return response.Metadata;
            }
            catch
            {
                _Log.Verbose("The object {0} does not exist in bucket {1}...", key, settings.BucketName);
                return null;
            }
        }

        /// <summary>
        /// Returns all the objects in a S3 bucket.
        /// </summary>
        /// <param name="prefix">Limits the response to keys that begin with the specified prefix.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async Task<IList<S3Object>> GetObjects(string prefix, S3Settings settings, CancellationToken cancellationToken = default(CancellationToken))
        {
            IList<S3Object> objects = new List<S3Object>();

            bool call = true;
            string marker = "";

            AmazonS3Client client = this.GetClient(settings);

            _Log.Verbose("Get objects from bucket {0}...", settings.BucketName);

            while (call)
            {
                ListObjectsRequest request = new ListObjectsRequest();
                request.BucketName = settings.BucketName;
                request.Marker = marker;

                if (!String.IsNullOrEmpty(prefix))
                {
                    request.Prefix = prefix;
                }

                ListObjectsResponse response = await client.ListObjectsAsync(request, cancellationToken);
                call = response.IsTruncated;
                marker = response.NextMarker;

                response.S3Objects.ForEach((lItem) =>
                {
                    objects.Add(lItem);
                });
            }

            return objects;
        }



        /// <summary>
        /// Generates a base64-encoded encryption key for Amazon S3 to use to encrypt / decrypt objects
        /// </summary>
        /// <param name="filePath">The file path to store the key in.</param>
        /// <param name="size">The size in bits of the secret key used by the symmetric algorithm</param>
        public void GenerateEncryptionKey(FilePath filePath, int size)
        {
            string fullPath = filePath.MakeAbsolute(_Environment.WorkingDirectory).FullPath;

            Aes aesEncryption = Aes.Create();
            aesEncryption.KeySize = size;
            aesEncryption.GenerateKey();

            string base64Key = Convert.ToBase64String(aesEncryption.Key);
            File.WriteAllText(fullPath, base64Key);
        }

        /// <summary>
        /// Create a signed URL allowing access to a resource that would usually require authentication. cts
        /// </summary>
        /// <param name="key">The key under which the Amazon S3 object is stored.</param>
        /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
        /// <param name="expires">The expiry date and time for the pre-signed url.</param>
        /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
        public string GetPreSignedURL(string key, string version, DateTime expires, S3Settings settings)
        {
            AmazonS3Client client = this.GetClient(settings);

            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest()
            {
                BucketName = settings.BucketName,
                Key = key,
                VersionId = version,
                Expires = expires
            };

            _Log.Verbose("Get object {0} from bucket {1}...", key, settings.BucketName);

            try
            {
                return client.GetPreSignedURL(request);
            }
            catch
            {
                _Log.Verbose("The object {0} does not exist in bucket {1}...", key, settings.BucketName);
                return "";
            }
        }
        #endregion
    }
}