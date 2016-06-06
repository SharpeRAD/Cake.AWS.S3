#region Using Statements
    using Cake.Core.IO;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// A path to upload to S3
    /// </summary>
    public class UploadPath
    {
        #region Constructor (1)
            /// <summary>
            /// Initializes a new instance of the <see cref="UploadPath" /> class.
            /// </summary>
            public UploadPath()
            {

            }
        #endregion





        #region Properties (3)
            /// <summary>
            /// The path to the file to upload
            /// </summary>
            public FilePath Path { get; set; }



            /// <summary>
            /// The key to store the S3 Objects under.
            /// </summary>
            public string Key { get; set; }

            /// <summary>
            /// The ETag to use when uploading the file
            /// </summary>
            public string ETag { get; set; }
        #endregion
    }
}
