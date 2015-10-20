#region Using Statements
    using Amazon.S3;
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
                CannedACL = S3CannedACL.Private;
                StorageClass = S3StorageClass.Standard;
            }
        #endregion





        #region Properties (2)
            /// <summary>
            /// The ACL to be used for S3 Buckets or S3 Objects.
            /// </summary>
            public S3CannedACL CannedACL { get; set; }

            /// <summary>
            /// Specifies the Storage Class of of an S3 object. Possible values are: ReducedRedundancy:
            ///  provides a 99.99% durability guarantee Standard: provides a 99.999999999% durability guarantee
            /// </summary>
            public S3StorageClass StorageClass { get; set; }
        #endregion
    }
}
