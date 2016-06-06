#region Using Statements
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    using Cake.Core;
    using Cake.Core.IO;
    using Cake.Core.Diagnostics;

    using Amazon.S3;
    using Amazon.S3.Model;
    using Amazon.S3.Transfer;

    using MimeSharp;
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
        #region Fields (4)
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





        #region Properties (1)
            /// <summary>
            /// If the manager should output progrtess events to the cake log
            /// </summary>
            public bool LogProgress { get; set; }
        #endregion





        #region Helper Functions (11)
            //Request
            private AmazonS3Client GetClient(S3Settings settings)
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
                if (settings.Region == null)
                {
                    throw new ArgumentNullException("settings.Region");
                }

                return new AmazonS3Client(settings.AccessKey, settings.SecretKey, settings.Region);
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



            //Hash
            private string GetHash(IFile file)
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = file.OpenRead())
                    {
                        return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-","").ToLower();
                    }
                }
            }
        #endregion





        #region Main Functions (13)
            /// <summary>
            /// Syncs the specified directory to Amazon S3, checking the modified date of the local fiels with existing S3Objects.
            /// </summary>
            /// <param name="dirPath">The directory path to sync to S3</param>
            /// <param name="settings">The <see cref="SyncSettings"/> required to sync to Amazon S3.</param>
            /// <returns>A list of keys that require invalidating.</returns>
            public IList<string> Sync(DirectoryPath dirPath, SyncSettings settings)
            {
                //Get Directory
                this.SetWorkingDirectory(settings);
                string fullPath = dirPath.MakeAbsolute(settings.WorkingDirectory).FullPath;

                IDirectory dir = _FileSystem.GetDirectory(dirPath.MakeAbsolute(settings.WorkingDirectory));
                List<string> list = new List<string>();

                if (settings.ModifiedCheck == ModifiedCheck.Hash)
                {
                    settings.GenerateETag = true;
                }



                if (dir.Exists)
                {
                    //Get S3 Objects
                    IList<S3Object> objects = this.GetObjects(settings.KeyPrefix, settings);

                    string prefix = settings.KeyPrefix;
                    if (String.IsNullOrEmpty(prefix))
                    {
                        prefix = "";
                    }
                    else if (!prefix.EndsWith("/"))
                    {
                        prefix = prefix + "/";
                    }



                    //Check Files
                    IEnumerable<IFile> files = dir.GetFiles(settings.SearchFilter, settings.SearchScope);
                    IList<UploadPath> upload = new List<UploadPath>();

                    foreach (IFile file in files)
                    {
                        //Get Key
                        string key;
                        if (settings.LowerPaths)
                        {
                            key = file.Path.FullPath.ToLower().Replace(fullPath.ToLower(), prefix.ToLower());
                        }
                        else
                        {
                            key = file.Path.FullPath.Replace(fullPath, prefix);
                        }

                        //Get ETag
                        string eTag = "";
                        if (settings.GenerateETag || (settings.ModifiedCheck == ModifiedCheck.Hash))
                        {
                            eTag = this.GetHash(file);
                        }



                        //Check Modified
                        S3Object obj = objects.FirstOrDefault(o => o.Key == key);

                        if ((obj == null) 
                            || ((settings.ModifiedCheck == ModifiedCheck.Hash) && (obj.ETag != eTag))
                            || ((settings.ModifiedCheck == ModifiedCheck.Date) && ((DateTimeOffset)new FileInfo(file.Path.FullPath).LastWriteTime) > (DateTimeOffset)obj.LastModified) )
                        {
                            upload.Add(new UploadPath()
                            {
                                Path = file.Path,
                                Key = key,
                                ETag = eTag
                            });

                            if (obj != null)
                            {
                                list.Add(key);
                            }
                        }

                        if (obj != null)
                        {
                            objects.Remove(obj);
                        }
                    }



                    //Upload
                    this.LogProgress = (upload.Count <= 1);
                    this.Upload(upload, settings);



                    //Delete
                    list.AddRange(objects.Select(o => o.Key).ToList());

                    this.Delete(objects.Select(o => o.Key).ToList(), settings);
                }

                return list;
            }



            /// <summary>
            /// Uploads a collection of files to S3. For large uploads, the file will be divided and uploaded in parts 
            /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
            /// </summary>
            /// <param name="paths">The paths to upload.</param>
            /// <param name="settings">The <see cref="UploadSettings"/> required to upload to Amazon S3.</param>
            public void Upload(IList<UploadPath> paths, SyncSettings settings)
            {
                Task.WhenAll(paths.Select(path => Task.Run(() =>
                {
                    UploadSettings copied = new UploadSettings()
                    {
                        WorkingDirectory = settings.WorkingDirectory,

                        AccessKey = settings.AccessKey,
                        SecretKey = settings.SecretKey,

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
                        GenerateETag = settings.GenerateETag
                    };

                    if (!String.IsNullOrEmpty(path.ETag))
                    {
                        copied.Headers["ETag"] = path.ETag;
                    }

                    this.Upload(path.Path, path.Key, copied);
                }))).Wait();
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

                //Set ContentType
                if (settings.GenerateContentType && String.IsNullOrEmpty(request.Headers.ContentType))
                {
                    request.Headers.ContentType = new Mime().Lookup(filePath.GetFilename().FullPath);
                }

                //Set ETag
                if (settings.GenerateETag && String.IsNullOrEmpty(request.Headers["ETag"]))
                {
                    request.Headers["ETag"] = this.GetHash(_FileSystem.GetFile(fullPath));
                }

                request.UploadProgressEvent += new EventHandler<UploadProgressArgs>(UploadProgressEvent);

                _Log.Verbose("Uploading file {0} to bucket {1}...", key, settings.BucketName);
                utility.UploadAsync(request);
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
            /// <param name="version">The identifier for the specific version of the object to be downloaded, if required.</param>
            /// <param name="settings">The <see cref="DownloadSettings"/> required to download from Amazon S3.</param>
            public void Download(FilePath filePath, string key, string version, DownloadSettings settings)
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
                utility.Download(request);
            }
        
            /// <summary>
            /// Opens a stream of the content from Amazon S3
            /// </summary>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="version">The identifier for the specific version of the object to be downloaded, if required.</param>
            /// <param name="settings">The <see cref="DownloadSettings"/> required to download from Amazon S3.</param>
            /// <returns>A stream.</returns>
            public Stream Open(string key, string version, DownloadSettings settings)
            {
                TransferUtility utility = this.GetUtility(settings);
                TransferUtilityOpenStreamRequest request = this.CreateOpenRequest(settings);

                request.Key = key;
                if (!String.IsNullOrEmpty(version))
                {
                    request.VersionId = version;
                }

                _Log.Verbose("Opening stream {0} from bucket {1}...", key, settings.BucketName);
                return utility.OpenStream(request);
            }

            /// <summary>
            /// Get the byte array of the content from Amazon S3
            /// </summary>
            /// <param name="key">The S3 object key.</param>
            /// <param name="version">The S3 object version.</param>
            /// <param name="settings">The download settings.</param>
            /// <returns>A byte array.</returns>
            public byte[] GetBytes(string key, string version, DownloadSettings settings)
            {
                byte[] data;

                using (Stream input = this.Open(key, version, settings))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        input.CopyTo(ms);
                        data = ms.ToArray();
                    }

                    input.Close();
                }

                return data;
            }



            /// <summary>
            /// Deletes a collection of files to S3. For large uploads, the file will be divided and uploaded in parts 
            /// using Amazon S3's multipart API. The parts will be reassembled as one object in Amazon S3.
            /// </summary>
            /// <param name="keys">The set of keys under which the Amazon S3 object is stored.</param>
            /// <param name="settings">The <see cref="S3Settings"/> required to upload to Amazon S3.</param>
            public void Delete(IList<string> keys, S3Settings settings)
            {
                Task.WhenAll(keys.Select(key => Task.Run(() =>
                {
                    this.Delete(key, "", settings);
                }))).Wait();
            }

            /// <summary>
            /// Removes the null version (if there is one) of an object and inserts a delete
            /// marker, which becomes the latest version of the object. If there isn't a null
            /// version, Amazon S3 does not remove any objects.
            /// </summary>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
            /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
            public void Delete(string key, string version, S3Settings settings)
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
                client.DeleteObject(request);
            }



            /// <summary>
            /// Retrieves object from Amazon S3.
            /// </summary>
            /// <param name="key">The key under which the Amazon S3 object is stored.</param>
            /// <param name="version">The identifier for the specific version of the object to be deleted, if required.</param>
            /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
            public S3Object GetObject(string key, string version, S3Settings settings)
            {
                AmazonS3Client client = this.GetClient(settings);
                GetObjectRequest request = this.CreateGetObjectRequest(key, version, settings);

                _Log.Verbose("Get object {0} from bucket {1}...", key, settings.BucketName);

                try
                {
                    GetObjectResponse response = client.GetObject(request);

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
            /// Returns all the objects in a S3 bucket.
            /// </summary>
            /// <param name="prefix">Limits the response to keys that begin with the specified prefix.</param>
            /// <param name="settings">The <see cref="S3Settings"/> required to download from Amazon S3.</param>
            public IList<S3Object> GetObjects(string prefix, S3Settings settings)
            {
                IList<S3Object> objects = new List<S3Object>();

                bool call = true;
                string marker = "";

                AmazonS3Client client = this.GetClient(settings);

                _Log.Verbose("Get objects from bucket {1}...", settings.BucketName);

                while (call)
                {
                    ListObjectsRequest request = new ListObjectsRequest();
                    request.BucketName = settings.BucketName;
                    request.Marker = marker;

                    if (!String.IsNullOrEmpty(prefix))
                    {
                        request.Prefix = prefix;
                    }

                    ListObjectsResponse response = client.ListObjects(request);
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
