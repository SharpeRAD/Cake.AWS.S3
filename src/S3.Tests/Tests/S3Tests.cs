#region Using Statements
    using System;

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
            IS3Manager manager = CakeHelper.CreateS3Manager();

            manager.Sync(new DirectoryPath("../../"), new SyncSettings()
            {
                AccessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID", EnvironmentVariableTarget.User),
                SecretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", EnvironmentVariableTarget.User),

                BucketName = "cake-aws-s3",
                KeyPrefix = "tests"
            });
        }
    }
}
