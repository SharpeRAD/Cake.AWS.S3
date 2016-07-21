#addin "Cake.AWS.S3"
#addin "Cake.AWS.CloudFront"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");



///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Sync-Directory")
    .Description("Syncs a directory to S3 using AWS Fallback credentials, requires Cake.AWS.CloudFront")
    .Does(() =>
{
    //Scan a local directory for files, comparing the contents against objects already in S3. Deleting missing objects and only uploading changed objects, returning a list of keys that require invalidating.
    var invalidate = S3Sync("./images/", Context.CreateSyncSettings()
    {
        BucketName = "cake-s3",

        SearchFilter = "*.png",
        SearchScope = SearchScope.Recursive,

        LowerPaths = true,
        KeyPrefix = "img/"
    });

    //Invalidate the list of keys that were either updated or deleted from the sync.
    CreateInvalidation("distribution", invalidate, Context.CreateCloudFrontSettings());
});



//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Sync-Directory");



///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);