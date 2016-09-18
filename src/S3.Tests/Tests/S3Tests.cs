#region Using Statements
    using System.Collections.Generic;

    using Xunit;

    using Cake.Core.IO;
#endregion



namespace Cake.AWS.S3.Tests
{
    public class S3Tests
    {
        [Fact]
        public void Test_Syn()
        {
            //Sync Directory
            SyncSettings settings = CakeHelper.CreateEnvironment().CreateSyncSettings();
            settings.BucketName = "cake-aws-s3";
            settings.KeyPrefix = "s3.tests";

            IS3Manager manager = CakeHelper.CreateS3Manager();
            IList<string> keys = manager.SyncUpload(new DirectoryPath("../../"), settings);
            
            Assert.NotEmpty(keys);
        }
    }
}