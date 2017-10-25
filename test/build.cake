#addin "Cake.AWS.S3"
#addin "Cake.AWS.CloudFront"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");





///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    //Executed BEFORE the first task.
    Information("Tools dir: {0}.", EnvironmentVariable("CAKE_PATHS_TOOLS"));
});





///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Sync-Directory")
    .Description("Syncs a directory to S3 using AWS Fallback credentials, requires Cake.AWS.CloudFront")
    .Does(async () =>
{
    //Scan a local directory for files, comparing the contents against objects already in S3. Deleting missing objects and only uploading changed objects, returning a list of keys that require invalidating.
    var settings = Context.CreateSyncSettings();

    settings.BucketName = "cake-s3";
    settings.SearchFilter = "*.png";
    settings.SearchScope = SearchScope.Recursive;
    settings.LowerPaths = true;
    settings.KeyPrefix = "img/";

    var invalidate = await S3SyncUpload(Directory("./images/"), settings);

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
