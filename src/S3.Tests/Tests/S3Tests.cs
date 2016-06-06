#region Using Statements
    using System;
    using System.Diagnostics;

    using Xunit;

    using Cake.Core;
    using Cake.Core.IO;
    using Cake.Core.Diagnostics;
#endregion



namespace Cake.AWS.S3.Tests
{
    public class S3Tests
    {
        [Fact]
        public void Test_Syn()
        {
            SyncSettings settings = CakeHelper.CreateEnvironment().CreateSyncSettings();
            settings.BucketName = "cake-aws-s3";
            settings.KeyPrefix = "tests";

            IS3Manager manager = CakeHelper.CreateS3Manager();
            manager.Sync(new DirectoryPath("../../"), settings);
        }
    }
}