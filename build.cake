#addin "Cake.Slack"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var appName = "Cake AWS.S3";





//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Get whether or not this is a local build.
var local = BuildSystem.IsLocalBuild;
var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;
var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;

// Parse release notes.
var releaseNotes = ParseReleaseNotes("./ReleaseNotes.md");

// Get version.
var buildNumber = AppVeyor.Environment.Build.Number;
var version = releaseNotes.Version.ToString();
var semVersion = local ? version : (version + string.Concat("-build-", buildNumber));

// Define directories.
var buildDir = "./src/S3/bin/" + configuration;
var buildResultDir = "./build/v" + semVersion;
var testResultsDir = buildResultDir + "/test-results";
var nugetRoot = buildResultDir + "/nuget";
var binDir = buildResultDir + "/bin";

//Get Solutions
var solutions       = GetFiles("./**/*.sln");





///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(() =>
{
	//Executed BEFORE the first task.
	Information("Building version {0} of {1}.", semVersion, appName);

	NuGetInstall("xunit.runner.console", new NuGetInstallSettings 
	{
		ExcludeVersion  = true,
		OutputDirectory = "./tools"
    });
});



Teardown(() =>
{
	// Executed AFTER the last task.
	Information("Finished building version {0} of {1}.", semVersion, appName);
});





///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
    // Clean solution directories.
	Information("Cleaning old files");
	CleanDirectories(new DirectoryPath[] 
	{
        buildResultDir, binDir, testResultsDir, nugetRoot
	});
});



Task("Restore-Nuget-Packages")
	.IsDependentOn("Clean")
    .Does(() =>
{
    // Restore all NuGet packages.
    foreach(var solution in solutions)
    {
        Information("Restoring {0}", solution);
        NuGetRestore(solution);
    }
});



Task("Patch-Assembly-Info")
    .IsDependentOn("Restore-Nuget-Packages")
    .Does(() =>
{
    var file = "./src/SolutionInfo.cs";

    CreateAssemblyInfo(file, new AssemblyInfoSettings {
		Product = appName,
        Version = version,
        FileVersion = version,
        InformationalVersion = semVersion,
        Copyright = "Copyright (c) Phillip Sharpe 2015"
    });
});



Task("Build")
    .IsDependentOn("Patch-Assembly-Info")
    .Does(() =>
{
    // Build all solutions.
    foreach(var solution in solutions)
    {
		Information("Building {0}", solution);
		MSBuild(solution, settings => 
			settings.SetPlatformTarget(PlatformTarget.MSIL)
				.WithProperty("TreatWarningsAsErrors","true")
				.WithTarget("Build")
				.SetConfiguration(configuration));
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit2("./src/**/bin/" + configuration + "/*.Tests.dll", new XUnit2Settings {
        OutputDirectory = testResultsDir,
        XmlReportV1 = true
    });
});



Task("Copy-Files")
    .IsDependentOn("Build")
    .Does(() =>
{
    CopyFileToDirectory(buildDir + "/Cake.Core.dll", binDir);
    CopyFileToDirectory(buildDir + "/Cake.Core.xml", binDir);

    CopyFileToDirectory(buildDir + "/Cake.AWS.S3.dll", binDir);
    CopyFileToDirectory(buildDir + "/Cake.AWS.S3.pdb", binDir);
    
    CopyFileToDirectory("./lib/AWSSDK.Core.dll", binDir);
    CopyFileToDirectory("./lib/AWSSDK.S3.dll", binDir);

    CopyFiles(new FilePath[] { "LICENSE", "README.md" }, binDir);



	CopyDirectory("./tools/",  "./test/tools/");
	CreateDirectory("./test/tools/Addins/Cake.AWS.S3/lib/net45/");

	CopyFileToDirectory(buildDir + "/Cake.AWS.S3.dll", "./test/tools/Addins/Cake.AWS.S3/lib/net45/");
	CopyFileToDirectory("./lib/AWSSDK.Core.dll", "./test/tools/Addins/Cake.AWS.S3/lib/net45/");
    CopyFileToDirectory("./lib/AWSSDK.S3.dll", "./test/tools/Addins/Cake.AWS.S3/lib/net45/");
});

Task("Zip-Files")
    .IsDependentOn("Copy-Files")
    .Does(() =>
{
    var filename = buildResultDir + "/Cake-AWS-S3-v" + semVersion + ".zip";
    Zip(binDir, filename);
});



Task("Create-NuGet-Packages")
    .IsDependentOn("Zip-Files")
    .Does(() =>
{
    NuGetPack("./nuspec/Cake.AWS.S3.nuspec", new NuGetPackSettings {
        Version = version,
        ReleaseNotes = releaseNotes.Notes.ToArray(),
        BasePath = binDir,
        OutputDirectory = nugetRoot,        
        Symbols = false,
        NoPackageAnalysis = true
    });
});



Task("Update-AppVeyor-Build-Number")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    AppVeyor.UpdateBuildVersion(semVersion);
}); 

Task("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Package")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    var artifact = new FilePath(buildResultDir + "/Cake-AWS-S3-v" + semVersion + ".zip");
    AppVeyor.UploadArtifact(artifact);
}); 



Task("Publish-Nuget")
	.IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => !local)
    .WithCriteria(() => !isPullRequest) 
    .Does(() =>
{
    // Resolve the API key.
    var apiKey = EnvironmentVariable("NUGET_API_KEY");
    if(string.IsNullOrEmpty(apiKey)) 
	{
        throw new InvalidOperationException("Could not resolve MyGet API key.");
    }

    // Get the path to the package.
    var package = nugetRoot + "/Cake.AWS.S3." + version + ".nupkg";

    // Push the package.
    NuGetPush(package, new NuGetPushSettings 
	{
        ApiKey = apiKey
    }); 
});



Task("Slack")
	.IsDependentOn("Create-NuGet-Packages")
    .Does(() =>
{
	var token = EnvironmentVariable("SLACK_TOKEN");
	var channel = "#code";
	var text = "Finished building version " + semVersion + " of " + appName;
	
	// Post the message.
	var result = Slack.Chat.PostMessage(token, channel, text);
	
	if (result.Ok)
	{
		//Posted
		Information("Message was succcessfully sent to Slack.");
	}
	else
	{
		//Error
		Error("Failed to send message to Slack: {0}", result.Error);
	}
});





//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Package")
	.IsDependentOn("Zip-Files")
    .IsDependentOn("Create-NuGet-Packages")
    .IsDependentOn("Slack");

Task("Default")
    .IsDependentOn("Package");

Task("AppVeyor")
    .IsDependentOn("Update-AppVeyor-Build-Number")
    .IsDependentOn("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Publish-Nuget");





///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);