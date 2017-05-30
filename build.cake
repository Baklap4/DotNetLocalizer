#addin "Cake.Powershell"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var versionSuffix = "nuget-test";

var isRunningOnAppVeyor = AppVeyor.IsRunningOnAppVeyor;

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./src/**/bin");
    CleanDirectories("./src/**/obj");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore("./src/DotNetLocalizer.Core");    
    DotNetCoreRestore("./src/DotNetLocalizer.Json");
    DotNetCoreRestore("./src/DotNetLocalizer.Yaml");
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        VersionSuffix = versionSuffix
    };

    DotNetCoreBuild("./src/DotNetLocalizer.Core", settings);
    DotNetCoreBuild("./src/DotNetLocalizer.Json", settings);
    DotNetCoreBuild("./src/DotNetLocalizer.Yaml", settings);
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    // TODO
});

Task("Version")
    .Does(() =>
{
    if (!isRunningOnAppVeyor)
    {
        throw new InvalidOperationException("Can only set version when running on AppVeyor");
    }

    var version = AppVeyor.Environment.Build.Version;

    StartPowershellFile("./update-version.ps1", args =>
    {
        args.Append("projectFile", "./src/DotNetLocalizer.Core/DotNetLocalizer.Core.csproj").Append("version", version);
    });
	StartPowershellFile("./update-version.ps1", args =>
    {
        args.Append("projectFile", "./src/DotNetLocalizer.Json/DotNetLocalizer.Json.csproj").Append("version", version);
    });
    StartPowershellFile("./update-version.ps1", args =>
    {
        args.Append("projectFile", "./src/DotNetLocalizer.Yaml/DotNetLocalizer.Yaml.csproj").Append("version", version);
    });
});

Task("Pack")
    .IsDependentOn("Version")
    .IsDependentOn("Build")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings
    {
        Configuration = "Release",
        OutputDirectory = "./output/"
    };

    DotNetCorePack("./src/DotNetLocalizer.Core", settings);
    DotNetCorePack("./src/DotNetLocalizer.Json", settings);
    DotNetCorePack("./src/DotNetLocalizer.Yaml", settings);
});

Task("Default")
    .IsDependentOn("Test");

RunTarget(target);