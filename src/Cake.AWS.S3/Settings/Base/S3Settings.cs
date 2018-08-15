#region Using Statements
using Amazon;
using Amazon.S3;
using Amazon.Runtime;

using Cake.Core.IO;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// The settings to use with requests to Amazon S3
    /// </summary>
    public abstract class S3Settings
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="S3Settings" /> class.
        /// </summary>
        public S3Settings()
        {
            Region = RegionEndpoint.EUWest1;

            EncryptionMethod = ServerSideEncryptionCustomerMethod.None;
        }
        #endregion





        #region Properties
        /// <summary>
        /// Gets or sets the working directory for the process to be started.
        /// </summary>
        public DirectoryPath WorkingDirectory { get; set; }



        /// <summary>
        /// The AWS Access Key ID
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// The AWS Secret Access Key.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// The AWS Session Token, if using temporary credentials.
        /// </summary>
        public string SessionToken { get; set; }
        
        internal AWSCredentials Credentials { get; set; }



        /// <summary>
        /// The endpoints available to AWS clients.
        /// </summary>
        public RegionEndpoint Region { get; set; }

        /// <summary>
        /// Gets or sets the name of the S3 bucket.
        /// </summary>
        public string BucketName { get; set; }



        /// <summary>
        /// The Server-side encryption algorithm to be used with the customer provided key.
        /// </summary>
        public ServerSideEncryptionCustomerMethod EncryptionMethod { get; set; }

        /// <summary>
        /// The base64-encoded encryption key for Amazon S3 to use to decrypt the object
        ///     Using the encryption key you provide as part of your request Amazon S3 manages
        ///     both the encryption, as it writes to disks, and decryption, when you access your
        ///     objects. Therefore, you don't need to maintain any data encryption code. The
        ///     only thing you do is manage the encryption keys you provide.
        ///     When you retrieve an object, you must provide the same encryption key as part
        ///     of your request. Amazon S3 first verifies the encryption key you provided matches,
        ///     and then decrypts the object before returning the object data to you.
        ///     Important: Amazon S3 does not store the encryption key you provide.
        /// </summary>
        public string EncryptionKey { get; set; }

        /// <summary>
        /// The MD5 of the customer encryption key. The MD5 is base 64 encoded. This field is optional, 
        /// the SDK will calculate the MD5 if this is not set.
        /// </summary>
        public string EncryptionKeyMD5 { get; set; }
        #endregion





        #region Methods
        /// <summary>
        /// Copies the settings to a instance of <see cref="S3Settings" /> class.
        /// </summary>
        protected T CopyS3Settings<T>(T copy = null) where T : S3Settings
        {
            copy.WorkingDirectory = this.WorkingDirectory;

            copy.AccessKey = this.AccessKey;
            copy.SecretKey = this.SecretKey;
            copy.SessionToken = this.SessionToken;
            copy.Credentials = this.Credentials;

            copy.Region = this.Region;
            copy.BucketName = this.BucketName;

            copy.EncryptionMethod = this.EncryptionMethod;
            copy.EncryptionKey = this.EncryptionKey;
            copy.EncryptionKeyMD5 = this.EncryptionKeyMD5;

            return copy;
        }
        #endregion
    }
}
