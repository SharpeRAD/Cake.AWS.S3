#region Using Statements
    using System;

    using Amazon;
    using Amazon.S3;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// Contains extension methods for <see cref="S3Settings" />.
    /// </summary>
    public static class S3SettingsExtensions
    {
        /// <summary>
        /// Specifies the AWS Access Key to use as credentials.
        /// </summary>
        /// <param name="settings">The S3 settings.</param>
        /// <param name="key">The AWS Access Key.</param>
        /// <returns>The same <see cref="S3Settings"/> instance so that multiple calls can be chained.</returns>
        public static T SetAccessKey<T>(this T settings, string key) where T : S3Settings
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            settings.AccessKey = key;
            return settings;
        }

        /// <summary>
        /// Specifies the AWS Secret Key to use as credentials.
        /// </summary>
        /// <param name="settings">The S3 settings.</param>
        /// <param name="key">The AWS Secret Access Key.</param>
        /// <returns>The same <see cref="S3Settings"/> instance so that multiple calls can be chained.</returns>
        public static T SetSecretKey<T>(this T settings, string key) where T : S3Settings
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            settings.SecretKey = key;
            return settings;
        }



        /// <summary>
        /// Specifies the endpoints available to AWS clients.
        /// </summary>
        /// <param name="settings">The S3 settings.</param>
        /// <param name="region">The endpoints available to AWS clients.</param>
        /// <returns>The same <see cref="S3Settings"/> instance so that multiple calls can be chained.</returns>
        public static T SetRegion<T>(this T settings, string region) where T : S3Settings
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (string.IsNullOrEmpty(region))
            {
                throw new ArgumentNullException("region");
            }

            settings.Region = RegionEndpoint.GetBySystemName(region);
            return settings;
        }

        /// <summary>
        /// Specifies the endpoints available to AWS clients.
        /// </summary>
        /// <param name="settings">The S3 settings.</param>
        /// <param name="region">The endpoints available to AWS clients.</param>
        /// <returns>The same <see cref="S3Settings"/> instance so that multiple calls can be chained.</returns>
        public static T SetRegion<T>(this T settings, RegionEndpoint region) where T : S3Settings
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }

            settings.Region = region;
            return settings;
        }



        /// <summary>
        /// Specifies the name of the load balancer.
        /// </summary>
        /// <param name="settings">The S3 settings.</param>
        /// <param name="name">The name of the S3 bucket.</param>
        /// <returns>The same <see cref="S3Settings"/> instance so that multiple calls can be chained.</returns>
        public static T SetBucketName<T>(this T settings, string name) where T : S3Settings
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            settings.BucketName = name;
            return settings;
        }



        /// <summary>
        /// Specifies the Server-side encryption algorithm to be used with the customer provided key.
        /// </summary>
        /// <param name="settings">The S3 settings.</param>
        /// <param name="method">The Server-side encryption algorithm to be used with the customer provided key.</param>
        /// <returns>The same <see cref="S3Settings"/> instance so that multiple calls can be chained.</returns>
        public static T SetEncryptionMethod<T>(this T settings, ServerSideEncryptionCustomerMethod method) where T : S3Settings
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            settings.EncryptionMethod = method;
            return settings;
        }

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
        /// <param name="settings">The S3 settings.</param>
        /// <param name="key">The base64-encoded encryption key for Amazon S3 to use to decrypt the object.</param>
        /// <returns>The same <see cref="S3Settings"/> instance so that multiple calls can be chained.</returns>
        public static T SetEncryptionKey<T>(this T settings, string key) where T : S3Settings
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            settings.EncryptionKey = key;
            return settings;
        }

        /// <summary>
        /// The MD5 of the customer encryption key. The MD5 is base 64 encoded. This field is optional, 
        /// the SDK will calculate the MD5 if this is not set.
        /// </summary>
        /// <param name="settings">The S3 settings.</param>
        /// <param name="md5">The MD5 of the customer encryption key..</param>
        /// <returns>The same <see cref="S3Settings"/> instance so that multiple calls can be chained.</returns>
        public static T SetEncryptionKeyMD5<T>(this T settings, string md5) where T : S3Settings
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (string.IsNullOrEmpty(md5))
            {
                throw new ArgumentNullException("md5");
            }

            settings.EncryptionKeyMD5 = md5;
            return settings;
        }
    }
}
