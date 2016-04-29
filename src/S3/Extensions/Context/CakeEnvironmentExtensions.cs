#region Using Statements
    using System;

    using Cake.Core;

    using Amazon;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// Contains extension methods for <see cref="ICakeEnvironment" />.
    /// </summary>
    public static class CakeEnvironmentExtensions
    {
        /// <summary>
        /// Helper method to get the AWS Credentials from environment variables
        /// </summary>
        /// <param name="environment">The cake environment.</param>
        /// <param name="settings">The S3 settings.</param>
        /// <returns>The same <see cref="S3Settings"/> instance so that multiple calls can be chained.</returns>
        private static T SetSettings<T>(this ICakeEnvironment environment, T settings) where T : S3Settings
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            settings.AccessKey = environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            settings.SecretKey = environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            settings.EncryptionKey = environment.GetEnvironmentVariable("AWS_ENCRYPTION_KEY");



            string region = environment.GetEnvironmentVariable("AWS_REGION");

            if (!String.IsNullOrEmpty(region))
            {
                settings.Region = RegionEndpoint.GetBySystemName(region);
            }

            return settings;
        }



        /// <summary>
        /// Helper method to get the AWS Credentials from environment variables
        /// </summary>
        /// <param name="environment">The cake environment.</param>
        /// <returns>A new <see cref="DownloadSettings"/> instance to be used in calls to the <see cref="IS3Manager"/>.</returns>
        public static DownloadSettings CreateDownloadSettings(this ICakeEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            return environment.SetSettings(new DownloadSettings());
        }

        /// <summary>
        /// Helper method to get the AWS Credentials from environment variables
        /// </summary>
        /// <param name="environment">The cake environment.</param>
        /// <returns>A new <see cref="DownloadSettings"/> instance to be used in calls to the <see cref="IS3Manager"/>.</returns>
        public static UploadSettings CreateUploadSettings(this ICakeEnvironment environment)
        {
            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            return environment.SetSettings(new UploadSettings());
        }
    }
}
