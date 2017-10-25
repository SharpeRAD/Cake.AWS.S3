# Cake.AWS.S3
Cake Build addin for transfering files to and from Amazon S3

[![Build status](https://ci.appveyor.com/api/projects/status/4ymtu0it99v31726?svg=true)](https://ci.appveyor.com/project/SharpeRAD/cake-aws-s3)

[![cakebuild.net](https://img.shields.io/badge/WWW-cakebuild.net-blue.svg)](http://cakebuild.net/)

[![Join the chat at https://gitter.im/cake-build/cake](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/cake-build/cake)



## Table of contents

1. [Implemented functionality](https://github.com/SharpeRAD/Cake.AWS.S3#implemented-functionality)
2. [Referencing](https://github.com/SharpeRAD/Cake.AWS.S3#referencing)
3. [Usage](https://github.com/SharpeRAD/Cake.AWS.S3#usage)
4. [Example](https://github.com/SharpeRAD/Cake.AWS.S3#example)
5. [Plays well with](https://github.com/SharpeRAD/Cake.AWS.S3#plays-well-with)
6. [License](https://github.com/SharpeRAD/Cake.AWS.S3#license)
7. [Share the love](https://github.com/SharpeRAD/Cake.AWS.S3#share-the-love)



## Implemented functionality

* Upload
* Download
* Open
* Delete
* ACL's
* Encryption
* PreSign URL
* Sync Upload / Download directory
* Uses AWS fallback credentials (app.config / web.config file, SDK store or credentials file, environment variables, instance profile)



## Referencing

[![NuGet Version](http://img.shields.io/nuget/v/Cake.AWS.S3.svg?style=flat)](https://www.nuget.org/packages/Cake.AWS.S3/)

Cake.AWS.S3 is available as a nuget package from the package manager console:

```csharp
Install-Package Cake.AWS.S3
```

or directly in your build script via a cake addin:

```csharp
#addin "Cake.AWS.S3"
```



## Usage

```csharp
#addin "Cake.AWS.S3"
#addin "Cake.AWS.CloudFront"

Task("Upload-File")
    .Description("Upload a file to S3")
    .Does(() =>
{
    S3Upload("C:/Files/test.zip", "test.zip", new UploadSettings()
    {
        AccessKey = "blah",
        SecretKey = "blah",

        Region = RegionEndpoint.EUWest1,
        BucketName = "cake-s3",

        CannedACL = S3CannedACL.Private,
        EncryptionKey = "mykey"
    });
});

Task("Download-File")
    .Description("Download a file from S3")
    .Does(() =>
{
    S3Download("C:/Files/test.zip", "test.zip", new DownloadSettings()
    {
        AccessKey = "blah",
        SecretKey = "blah",

        Region = RegionEndpoint.EUWest1,
        BucketName = "cake-s3",

        CannedACL = S3CannedACL.Private,
        EncryptionKey = "mykey"
    });
});



Task("Upload-File-Fluent")
    .Description("Upload a file to S3")
    .Does(() =>
{
    S3Upload("C:/Files/test.zip", "test.zip",
        new UploadSettings()
            .SetAccessKey("blah")
            .SetSecretKey("blah")

            .SetRegion("eu-west-1")
            .SetBucketName("cake-s3")

            .SetCannedACL(S3CannedACL.Private)
            .SetEncryptionKey("mykey"));
});

Task("Download-File-Fallback")
    .Description("Download a file from S3 using AWS Fallback credentials")
    .Does(() =>
{
    var settings = Context.CreateDownloadSettings(); 
    settings.BucketName = "cake-s3";

    S3Download("C:/Files/test.zip", "test.zip", settings);
});



Task("Sync-Directory-To-S3")
    .Description("Syncs a directory to S3 using AWS Fallback credentials (requires Cake.AWS.CloudFront for invalidation)")
    .Does(() =>
{
    //Scan a local directory for files, comparing the contents against objects already in S3. Deleting missing objects and only uploading changed objects, returning a list of keys that require invalidating.
    var invalidate = S3SyncUpload("./images/", Context.CreateSyncSettings()
    {
        BucketName = "cake-s3",

        SearchFilter = "*.png",
        SearchScope = SearchScope.Recursive,

        //Default content type is used when file has no extension or the content type can't be generated using extension
        DefaultContentType = "text/html",

        LowerPaths = true,
        KeyPrefix = "img/",

        //Compares MD5 hash or modified date
        ModifiedCheck = ModifiedCheck.Hash
    });

    //Invalidate the list of keys that were either updated or deleted from the sync.
    CreateInvalidation("distribution", invalidate, Context.CreateCloudFrontSettings());
});

Task("Sync-Directory-From-S3")
    .Description("Syncs a directory from S3 using AWS Fallback credentials, please be aware this deletes missing files!")
    .Does(() =>
{
    //Scan a local directory for files, comparing the contents against objects already in S3. Deleting missing files and only downloading changed objects.
    var invalidate = S3SyncDownload("./images/", Context.CreateSyncSettings()
    {
        BucketName = "cake-s3",

        SearchFilter = "*.png",
        SearchScope = SearchScope.Recursive,

        LowerPaths = true,
        KeyPrefix = "img/",

        //Compares MD5 hash or modified date
        ModifiedCheck = ModifiedCheck.Hash
    });
});



Task("Generate-Encryption-Key")
    .Description("Helper method to generate an encryption key")
    .Does(() =>
{
    GenenrateEncryptionKey("./Key.txt");
});

RunTarget("Upload-File");
```



## Example

A complete Cake example can be found [here](https://github.com/SharpeRAD/Cake.AWS.S3/blob/master/test/build.cake).



## TroubleShooting

* Please be aware of the breaking changes that occurred with the release of [Cake v0.22.0](https://cakebuild.net/blog/2017/09/cake-v0.22.0-released), you will need to upgrade Cake in order to use Cake.AWS.S3 v0.5.0 or above.

* Please be aware of the breaking changes that occurred with the release of Cake.AWS.S3 v0.6.0, in order to support netstandard1.6 I had to switch to using async methods.



## Plays well with

If your S3 buckets are linked to CloudFront distributions its worth checking out [Cake.AWS.CloudFront](https://github.com/SharpeRAD/Cake.AWS.CloudFront).

If your looking for a way to trigger cake tasks based on windows events or at scheduled intervals then check out [CakeBoss](https://github.com/SharpeRAD/CakeBoss).



## License

Copyright (c) 2015 - 2016 Phillip Sharpe

Cake.AWS.S3 is provided as-is under the MIT license. For more information see [LICENSE](https://github.com/SharpeRAD/Cake.AWS.S3/blob/master/LICENSE).



## Share the love

If this project helps you in anyway then please :star: the repository.
