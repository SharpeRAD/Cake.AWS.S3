### New in 0.5.0 (Released 2017/10/07)
* [Improvement] Updated Cake reference to v0.22.0
* [Improvement] Moved to net461 for Cake compatibility
* [Improvement] Upgraded solution to vs2017
* [Improvement] New .net core based build scripts

### New in 0.4.7 (Released 2017/09/11)
* [Improvement] Use MimeTypesMap over MimeSharp

### New in 0.4.6 (Released 2017/06/29)
* [Improvement] Added DefaultContentType to UploadSettings

### New in 0.4.5 (Released 2017/02/21)
* [Bug] Dont GenerateContentLength for multi-part uploads

### New in 0.4.4 (Released 2017/02/21)
* [Bug] SyncUpload wasnt copying the full settings

### New in 0.4.3 (Released 2017/02/20)
* [Bug] DeleteAll skipping files when lastModified wasn't set

### New in 0.4.2 (Released 2017/02/18)
* [Feature] Add CacheControl header to UploadSettings

### New in 0.4.1 (Released 2017/02/18)
* [Improvement] Move compression extension list to UploadSettings

### New in 0.4.0 (Released 2017/02/03)
* [Bug] Fix SyncUpload blank key

### New in 0.3.9 (Released 2017/01/05)
* [Bug] Fix prefix and add optional compression setting

### New in 0.3.8 (Released 2017/01/04)
* [Improvement] Update packages

### New in 0.3.7 (Released 2016/12/21)
* [Improvement] Generate Content-Length header for uploads

### New in 0.3.6 (Released 2016/12/09)
* [Improvement] Custom meta tags need to be prefixed with "x-amz-meta-hashtag"

### New in 0.3.5 (Released 2016/12/07)
* [Bug] Fix hash tag

### New in 0.3.4 (Released 2016/12/07)
* [Feature] Add GetHashTag alias to store custom hash tag to get around AWS multi-part etag bug

### New in 0.3.3 (Released 2016/12/03)
* [Bug] GetS3String stack overflow

### New in 0.3.2 (Released 2016/12/03)
* [Feature] GetS3String alias
* [Feature] S3GetTag alias
* [Feature] GetFileHash alias

### New in 0.3.1 (Released 2016/11/08)
* [Feature] DeleteAll objects
* [Bug] Fixed KeyPrefix in Sync methods

### New in 0.3.0 (Released 2016/11/07)
* [Improvement] Don't log progress when calling sync methods

### New in 0.2.9 (Released 2016/10/05)
* [Feature] Sync file aliases

### New in 0.2.8 (Released 2016/09/18)
* [Improvement] Return uploaded keys from sync

### New in 0.2.7 (Released 2016/09/17)
* [Bug] Fixed SyncDownload implementation

### New in 0.2.6 (Released 2016/09/11)
* Added SyncDownlad method and renamed sync to SyncUpload

### New in 0.2.5 (Released 2016/08/31)
* [Bug] Fix sync credentials

### New in 0.2.4 (Released 2016/08/30)
* [Bug] Fix sync keys

### New in 0.2.3 (Released 2016/08/16)
* [Bug] Fix Fallback credential implimentation

### New in 0.2.2 (Released 2016/06/06)
* [Improvement] Generate Etag
* [Improvement] Sync based on Etag or modified date
* [Improvement] Add unit-test
* [Improvement] LogProgress setting

### New in 0.2.1 (Released 2016/06/02)
* [Improvement] Add HeadersCollection to upload settings
* [Feature] Use MimeSharp to set the content-type of uploaded objects
* [Improvement] Rename ModifiedDate to LastModified and change to DateTimeOffset
* [Feature] Sync alias

### New in 0.2.0 (Released 2016/05/21)
* [Improvement] Add KeyManagementServiceKeyId to UploadSettings

### New in 0.1.9 (Released 2016/05/17)
* [Improvement] Use AWS FallbackCredentialsFactory
* [Improvement] Add nuget dependencies
* [Improvement] Update all packages
* [Improvement] Setup and Teardown with context

### New in 0.1.8 (Released 2016/04/29)
* [Improvement] Match the environment variables used in the AWS SDK

### New in 0.1.7 (Released 2016/02/29)
* [Improvement] Add prefix to GetObjects to limit results
* [Improvement] Rename GetObject to "GetS3Object"

### New in 0.1.6 (Released 2016/02/27)
* [Feature] GetBytes

### New in 0.1.5 (Released 2016/02/18)
* [Feature] Open stream

### New in 0.1.4 (Released 2016/01/16)
* [Improvement] Fixed SolutionInfo link

### New in 0.1.3 (Released 2016/01/08)
* [Improvement] Download alias without version

### New in 0.1.2 (Released 2016/01/06)
* [Bug] Fix bug in GetPreSignedURL

### New in 0.1.1 (Released 2015/12/28)
* [Improvement] Add xml documentation

### New in 0.1.0 (Released 2015/12/10)
* [Improvement] Add Cake namespace docs
* [Improvement] Update Cake.Core reference

### New in 0.0.9 (Released 2015/11/30)
* [Feature] GetObject and GetObjects

### New in 0.0.8 (Released 2015/10/27)
* [Feature] Get Pre-Signed Url
* [Improvement] Rename TransferManager to S3Manager

### New in 0.0.7 (Released 2015/10/24)
* [Improvement] Extra argument checks
* [Improvement] Update Nuget packages

### New in 0.0.6 (Released 2015/09/29)
* [Feature] Delete aliase
* [Feature] LastModified aliase

### New in 0.0.5 (Released 2015/09/28)
* [Improvement] Remove Cake.Core reference
* [Improvement] Add AliaseCategories

### New in 0.0.4 (Released 2015/09/23)
* [Improvement] Use FilePath and WorkingDirectory for file locations
* [Improvement] Add bucket and keys to Verbose logs about the transfer details
* [Improvement] Add GenenrateEncryptionKey alias

### New in 0.0.3 (Released 2015/09/22)
* [Improvement] Logging Progress-Events

### New in 0.0.2 (Released 2015/09/18)
* [Improvement] Extensions methods for settings

### New in 0.0.1 (Released 2015/09/16)
* [Feature] First release.
