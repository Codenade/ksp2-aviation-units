var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");

Task("Clean")
	.WithCriteria(c => HasArgument("rebuild"))
	.Does(() =>
	{
		CleanDirectory($"./build");
	});

Task("Build")
	.IsDependentOn("Clean")
	.Does(() =>
	{
		DotNetBuild("./ksp2-aviation-units.sln", new DotNetBuildSettings
		{
			Configuration = configuration
		});
	});

RunTarget(target);