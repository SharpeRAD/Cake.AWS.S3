#region Using Statements
    using System;

    using Cake.Core.IO;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// Contains extension methods for <see cref="SyncSettings" />.
    /// </summary>
    public static class SyncSettingsExtensions
    {
        /// <summary>
        /// Specifies the filter to use when searching for files.
        /// </summary>
        /// <param name="settings">The sync settings.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>The same <see cref="SyncSettings"/> instance so that multiple calls can be chained.</returns>
        public static SyncSettings SetSearchFilter(this SyncSettings settings, string filter)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.SearchFilter = filter;
            return settings;
        }

        /// <summary>
        /// Specifies the scope to use when searching for files.
        /// </summary>
        /// <param name="settings">The sync settings.</param>
        /// <param name="scope">The scope.</param>
        /// <returns>The same <see cref="SyncSettings"/> instance so that multiple calls can be chained.</returns>
        public static SyncSettings SetSearchScope(this SyncSettings settings, SearchScope scope = SearchScope.Current)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.SearchScope = scope;
            return settings;
        }



        /// <summary>
        /// Lower the file paths when generating S3 keys
        /// </summary>
        /// <param name="settings">The sync settings.</param>
        /// <param name="lowerPaths">lower file paths.</param>
        /// <returns>The same <see cref="SyncSettings"/> instance so that multiple calls can be chained.</returns>
        public static SyncSettings SetLowerPaths(this SyncSettings settings, bool lowerPaths = false)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.LowerPaths = lowerPaths;
            return settings;
        }

        /// <summary>
        /// The prefix to use when generating S3 keys
        /// </summary>
        /// <param name="settings">The sync settings.</param>
        /// <param name="keyPrefix">the key prefix.</param>
        /// <returns>The same <see cref="SyncSettings"/> instance so that multiple calls can be chained.</returns>
        public static SyncSettings SetKeyPrefix(this SyncSettings settings, string keyPrefix)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.KeyPrefix = keyPrefix;
            return settings;
        }
        


        /// <summary>
        /// The prefix to use when generating S3 keys
        /// </summary>
        /// <param name="settings">The sync settings.</param>
        /// <param name="modifiedCheck">the modified check.</param>
        /// <returns>The same <see cref="SyncSettings"/> instance so that multiple calls can be chained.</returns>
        public static SyncSettings SetModifiedCheck(this SyncSettings settings, ModifiedCheck modifiedCheck = ModifiedCheck.Date)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.ModifiedCheck = modifiedCheck;
            return settings;
        }
    }
}
