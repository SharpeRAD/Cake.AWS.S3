#region Using Statements
using Cake.Core.IO;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// The settings to use when syncing a folder to Amazon S3
    /// </summary>
    public class SyncSettings : UploadSettings
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncSettings" /> class.
        /// </summary>
        public SyncSettings()
        {
            SearchFilter = "*";
            SearchScope = SearchScope.Recursive;

            LowerPaths = true;
            KeyPrefix = "";
            ModifiedCheck = ModifiedCheck.Hash;
        }
        #endregion





        #region Properties
        /// <summary>
        /// The filter to use when searching for files
        /// </summary>
        public string SearchFilter { get; set; }

        /// <summary>
        /// The scope to use when searching for files
        /// </summary>
        public SearchScope SearchScope { get; set; }



        /// <summary>
        /// Lower the file paths when generating S3 keys
        /// </summary>
        public bool LowerPaths { get; set; }

        /// <summary>
        /// The prefix to use when generating S3 keys
        /// </summary>
        public string KeyPrefix { get; set; }
        


        /// <summary>
        /// How to check if a file has been modified
        /// </summary>
        public ModifiedCheck ModifiedCheck { get; set; }
        #endregion





        #region Methods
        /// <summary>
        /// Creates a new instance of the <see cref="SyncSettings" /> class with the current settings.
        /// </summary>
        public new SyncSettings Clone()
        {
            return this.CopySyncSettings(new SyncSettings());
        }

        /// <summary>
        /// Copies the settings to a instance of <see cref="UploadSettings" /> class.
        /// </summary>
        protected T CopySyncSettings<T>(T copy = null) where T : SyncSettings
        {
            this.CopyUploadSettings(copy);

            copy.SearchFilter = this.SearchFilter;
            copy.SearchScope = this.SearchScope;

            copy.LowerPaths = this.LowerPaths;
            copy.KeyPrefix = this.KeyPrefix;

            copy.ModifiedCheck = this.ModifiedCheck;

            return copy;
        }
        #endregion
    }
}
