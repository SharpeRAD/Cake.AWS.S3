﻿#region Using Statements
using System;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// The settings to use with downlad requests to Amazon S3
    /// </summary>
    public class DownloadSettings : S3Settings
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadSettings" /> class.
        /// </summary>
        public DownloadSettings()
        {
            ModifiedDate = DateTime.MinValue;
        }
        #endregion





        #region Properties
        /// <summary>
        /// The date the file was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; }
        #endregion





        #region Methods
        /// <summary>
        /// Creates a new instance of the <see cref="DownloadSettings" /> class with the current settings.
        /// </summary>
        public DownloadSettings Copy()
        {
            DownloadSettings copy = this.CopyS3Settings(new DownloadSettings());

            copy.ModifiedDate = this.ModifiedDate;

            return copy;
        }
        #endregion
    }
}
