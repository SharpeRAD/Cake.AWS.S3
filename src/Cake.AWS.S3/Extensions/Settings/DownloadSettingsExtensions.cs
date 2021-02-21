#region Using Statements
using System;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// Contains extension methods for <see cref="DownloadSettings" />.
    /// </summary>
    public static class DownloadSettingsExtensions
    {
        /// <summary>
        /// Specifies the date the file was last modified
        /// </summary>
        /// <param name="settings">The Download settings.</param>
        /// <param name="modifiedDate">The modified date.</param>
        /// <returns>The same <see cref="DownloadSettings"/> instance so that multiple calls can be chained.</returns>
        public static DownloadSettings SetModifiedDate(this DownloadSettings settings, DateTime modifiedDate)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.ModifiedDate = modifiedDate;
            return settings;
        }
    }
}
