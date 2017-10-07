#region Using Statements
using System;

using Cake.Core;

using Amazon;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// Contains extension methods for <see cref="ICakeContext" />.
    /// </summary>
    public static class CakeContextExtensions
    {
        /// <summary>
        /// Helper method to get the AWS Credentials from environment variables
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <returns>A new <see cref="DownloadSettings"/> instance to be used in calls to the <see cref="IS3Manager"/>.</returns>
        public static DownloadSettings CreateDownloadSettings(this ICakeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.Environment.CreateDownloadSettings();
        }

        /// <summary>
        /// Helper method to get the AWS Credentials from environment variables
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <returns>A new <see cref="DownloadSettings"/> instance to be used in calls to the <see cref="IS3Manager"/>.</returns>
        public static UploadSettings CreateUploadSettings(this ICakeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.Environment.CreateUploadSettings();
        }
        
        /// <summary>
        /// Helper method to get the AWS Credentials from environment variables
        /// </summary>
        /// <param name="context">The cake context.</param>
        /// <returns>A new <see cref="SyncSettings"/> instance to be used in calls to the <see cref="IS3Manager"/>.</returns>
        public static SyncSettings CreateSyncSettings(this ICakeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            return context.Environment.CreateSyncSettings();
        }
    }
}
