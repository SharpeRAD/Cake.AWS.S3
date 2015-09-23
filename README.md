# Cake.AWS.S3
Cake Build addon for transfering files to and from Amazon S3

[![Build status](https://ci.appveyor.com/api/projects/status/4ymtu0it99v31726?svg=true)](https://ci.appveyor.com/project/PhillipSharpe/cake-aws-s3)

[![cakebuild.net](https://img.shields.io/badge/WWW-cakebuild.net-blue.svg)](http://cakebuild.net/)

[![Join the chat at https://gitter.im/cake-build/cake](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/cake-build/cake?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)



## Implemented functionality

* Upload
* Download
* ACL's
* Encryption



## Referencing

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

Task("Download-File-Environment")
    .Description("Download a file from S3")
    .Does(() =>
{
    S3Download("C:/Files/test.zip", "test.zip", Context.CreateDownloadSettings()
    {
        BucketName = "cake-s3"
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

A complete Cake example can be found [here](https://github.com/SharpeRAD/Cake.AWS.S3/blob/master/test/build.cake)
