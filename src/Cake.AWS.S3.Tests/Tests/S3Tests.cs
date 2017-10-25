#region Using Statements
using System.Threading.Tasks;
using System.Collections.Generic;

using Xunit;
using Shouldly;
using Amazon.S3.Model;

using Cake.Core.IO;
#endregion



namespace Cake.AWS.S3.Tests
{
    public class S3Tests
    {
        [Fact]
        public async Task Test_Syn()
        {
            //Sync Directory
            SyncSettings settings = CakeHelper.CreateEnvironment().CreateSyncSettings();
            settings.BucketName = "cake-aws-s3";
            settings.KeyPrefix = "s3.tests";

            IS3Manager manager = CakeHelper.CreateS3Manager();
            IList<string> keys = await manager.SyncUpload(new DirectoryPath("./Files"), settings);
            
            keys.ShouldBeEmpty();
        }

        [Fact]
        public async Task Test_Upload_ContentLength()
        {
            //Upload
            UploadSettings settings = CakeHelper.CreateEnvironment().CreateUploadSettings();
            settings.BucketName = "cake-aws-s3";

            settings.GenerateContentLength = true;
            settings.CompressContent = true;
            settings.CannedACL = Amazon.S3.S3CannedACL.PublicRead;
            settings.CacheControl = "private, max-age=86400";

            IS3Manager manager = CakeHelper.CreateS3Manager();
            await manager.Upload(new FilePath("./Files/Test.css"), "Tester.css", settings);
        }



        [Fact]
        public async Task Test_Meta()
        {
            //Upload
            UploadSettings settings = CakeHelper.CreateEnvironment().CreateUploadSettings();
            settings.BucketName = "cake-aws-s3";

            IS3Manager manager = CakeHelper.CreateS3Manager();
            await manager.Upload(new FilePath("./Files/Encoding.txt"), "Encodings.txt", settings);



            //Get Meta
            MetadataCollection meta = await manager.GetObjectMetaData("Encodings.txt", "", settings);

            string metaHash = meta["x-amz-meta-hashtag"];
        }
    }
}