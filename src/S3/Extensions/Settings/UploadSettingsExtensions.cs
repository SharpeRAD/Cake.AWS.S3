#region Using Statements
    using System;

    using Amazon.S3;
#endregion



namespace Cake.AWS.S3
{
    /// <summary>
    /// Contains extension methods for <see cref="UploadSettings" />.
    /// </summary>
    public static class UploadSettingsExtensions
    {
        /// <summary>
        /// Specifies the ACL to be used for S3 Buckets or S3 Objects.
        /// </summary>
        /// <param name="settings">The upload settings.</param>
        /// <param name="cannedACL">The canned ACL.</param>
        /// <returns>The same <see cref="UploadSettings"/> instance so that multiple calls can be chained.</returns>
        public static UploadSettings SetCannedACL(this UploadSettings settings, S3CannedACL cannedACL)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (cannedACL == null)
            {
                throw new ArgumentNullException("cannedACL");
            }

            settings.CannedACL = cannedACL;
            return settings;
        }

        /// <summary>
        /// Specifies the ACL to be used for S3 Buckets or S3 Objects.
        /// </summary>
        /// <param name="settings">The upload settings.</param>
        /// <param name="cannedACL">The canned ACL name.</param>
        /// <returns>The same <see cref="UploadSettings"/> instance so that multiple calls can be chained.</returns>
        public static UploadSettings SetCannedACL(this UploadSettings settings, string cannedACL)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (string.IsNullOrEmpty(cannedACL))
            {
                throw new ArgumentNullException("cannedACL");
            }

            settings.CannedACL = S3CannedACL.FindValue(cannedACL);
            return settings;
        }



        /// <summary>
        /// Specifies the Storage Class of of an S3 object. Possible values are: ReducedRedundancy:
        ///  provides a 99.99% durability guarantee Standard: provides a 99.999999999% durability guarantee
        /// </summary>
        /// <param name="settings">The upload settings.</param>
        /// <param name="storageClass">The storage class.</param>
        /// <returns>The same <see cref="UploadSettings"/> instance so that multiple calls can be chained.</returns>
        public static UploadSettings SetStorageClass(this UploadSettings settings, S3StorageClass storageClass)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (storageClass == null)
            {
                throw new ArgumentNullException("storageClass");
            }

            settings.StorageClass = storageClass;
            return settings;
        }

        /// <summary>
        /// Specifies the Storage Class of of an S3 object. Possible values are: ReducedRedundancy:
        ///  provides a 99.99% durability guarantee Standard: provides a 99.999999999% durability guarantee
        /// </summary>
        /// <param name="settings">The upload settings.</param>
        /// <param name="storageClass">The storage class.</param>
        /// <returns>The same <see cref="UploadSettings"/> instance so that multiple calls can be chained.</returns>
        public static UploadSettings SetStorageClass(this UploadSettings settings, string storageClass)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (string.IsNullOrEmpty(storageClass))
            {
                throw new ArgumentNullException("storageClass");
            }

            settings.StorageClass = S3StorageClass.FindValue(storageClass);
            return settings;
        }
        
        /// <summary>
        /// The id of the AWS Key Management Service key that Amazon S3 should use to encrypt
        /// and decrypt the object. If a key id is not specified, the default key will be
        /// </summary>
        /// <param name="settings">The upload settings.</param>
        /// <param name="id">The id of the key tp use.</param>
        /// <returns>The same <see cref="UploadSettings"/> instance so that multiple calls can be chained.</returns>
        public static UploadSettings SetKeyManagementServiceKeyId(this UploadSettings settings, string id)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }

            settings.KeyManagementServiceKeyId = id;
            return settings;
        }
        

        
        /// <summary>
        /// Generate the ContentType based on the file extension
        /// </summary>
        /// <param name="settings">The sync settings.</param>
        /// <param name="generateContentType">generate ContentType.</param>
        /// <returns>The same <see cref="UploadSettings"/> instance so that multiple calls can be chained.</returns>
        public static UploadSettings SetGenerateContentType(this UploadSettings settings, bool generateContentType = true)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.GenerateContentType = generateContentType;
            return settings;
        }

        /// <summary>
        /// Generate an ETag based on the hash of the file
        /// </summary>
        /// <param name="settings">The sync settings.</param>
        /// <param name="generateETag">generate ETag.</param>
        /// <returns>The same <see cref="UploadSettings"/> instance so that multiple calls can be chained.</returns>
        public static UploadSettings SetGenerateETag(this UploadSettings settings, bool generateETag = true)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            settings.GenerateETag = generateETag;
            return settings;
        }
    }
}
