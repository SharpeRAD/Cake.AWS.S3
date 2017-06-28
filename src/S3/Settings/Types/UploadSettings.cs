#region Using Statements
    using System.Collections.Generic;

    using Amazon.S3;
    using Amazon.S3.Model;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// The settings to use with upload requests to Amazon S3
    /// </summary>
    public class UploadSettings : S3Settings
    {
        #region Constructor (1)
            /// <summary>
            /// Initializes a new instance of the <see cref="UploadSettings" /> class.
            /// </summary>
            public UploadSettings()
            {
                this.CannedACL = S3CannedACL.Private;
                this.StorageClass = S3StorageClass.Standard;
                this.KeyManagementServiceKeyId = "";

                this.Headers = new HeadersCollection();

                this.GenerateContentType = true;
                this.GenerateContentLength = false;

                this.GenerateETag = true;
                this.GenerateHashTag = true;

                this.CompressContent = false;
                this.CompressExtensions = new List<string>()
                {
                    ".css",
                    ".js",
                    ".json"
                };
            }
        #endregion





        #region Properties (7)
            /// <summary>
            /// The ACL to be used for S3 Buckets or S3 Objects.
            /// </summary>
            public S3CannedACL CannedACL { get; set; }

            /// <summary>
            /// Specifies the Storage Class of of an S3 object. Possible values are: ReducedRedundancy:
            /// provides a 99.99% durability guarantee Standard: provides a 99.999999999% durability guarantee
            /// </summary>
            public S3StorageClass StorageClass { get; set; }


        
            /// <summary>
            /// The id of the AWS Key Management Service key that Amazon S3 should use to encrypt
            /// and decrypt the object. If a key id is not specified, the default key will be
            /// </summary>
            public string KeyManagementServiceKeyId { get; set; }


                
            /// <summary>
            /// Used to set the http-headers for an S3 object.
            /// </summary>
            public HeadersCollection Headers { get; set; }
        

                
            /// <summary>
            /// Generate the ContentType based on the file extension
            /// </summary>
            public bool GenerateContentType { get; set; }

            /// <summary>
            /// Content type to use when no mime type is found
            /// </summary>
            public string DefaultContentType { get; set; }

            /// <summary>
            /// Generate the ContentLength based on the file size in bytes
            /// </summary>
            public bool GenerateContentLength { get; set; }

            /// <summary>
            /// Generate an ETag based on the hash of the file
            /// </summary>
            public bool GenerateETag { get; set; }
        
            /// <summary>
            /// Generate a custom meta-data field based on the hash of the file
            /// </summary>
            public bool GenerateHashTag { get; set; }


                        
            /// <summary>
            /// Gzip the content of css / js
            /// </summary>
            public bool CompressContent { get; set; }
                                
            /// <summary>
            /// List of file extensions to compress
            /// </summary>
            public IList<string> CompressExtensions { get; set; }


                                        
            /// <summary>
            /// How objects should be cached
            /// </summary>
            public string CacheControl { get; set; }
        #endregion
    }
}