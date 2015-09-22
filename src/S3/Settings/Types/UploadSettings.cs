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
            }
        #endregion





        #region Properties (1)
            /// <summary>
            /// The ACL to be used for S3 Buckets or S3 Objects.
            /// </summary>
            public S3CannedACL CannedACL { get; set; }
        #endregion
    }
}
