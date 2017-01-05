#region Using Statements
    using System.Collections.Generic;

    using Xunit;
    using Amazon.S3.Model;

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

        [Fact]
        public void Test_Upload_ContentLength()
        {
            //Upload
            UploadSettings settings = CakeHelper.CreateEnvironment().CreateUploadSettings();
            settings.BucketName = "cake-aws-s3";

            settings.GenerateContentLength = true;
            settings.CompressContent = true;
            settings.CannedACL = Amazon.S3.S3CannedACL.PublicRead;

            IS3Manager manager = CakeHelper.CreateS3Manager();
            manager.Upload(new FilePath("../../../Test.css"), "Tester.css", settings);
        }



        [Fact]
        public void Test_Meta()
        {
            //Upload
            UploadSettings settings = CakeHelper.CreateEnvironment().CreateUploadSettings();
            settings.BucketName = "cake-aws-s3";

            IS3Manager manager = CakeHelper.CreateS3Manager();
            manager.Upload(new FilePath("../../packages.config"), "packages.config", settings);



            //Get Meta
            MetadataCollection meta = manager.GetObjectMetaData("packages.config", "", settings);

            string metaHash = meta["x-amz-meta-hashtag"];
        }
    }
}