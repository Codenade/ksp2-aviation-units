#addin nuget:?package=Cake.Unity&version=0.9.0

var target = Argument("target", "Pack");
var configuration = Argument("configuration", "Release");
var ksp2Root = Argument("ksp2-root", string.Empty);

Task("Clean")
	.Does(() =>
	{
		CleanDirectory($"./build");
	});

Task("Build")
	.IsDependentOn("Clean")
	.Does(() =>
	{
		CreateDirectory("./build/BepInEx/plugins/ksp2-aviation-units/");
		DotNetBuild("./ksp2-aviation-units.sln", new DotNetBuildSettings
		{
			Configuration = configuration
		});
		CopyFileToDirectory($"./ksp2-aviation-units/bin/{configuration}/netstandard2.0/ksp2-aviation-units.dll", Directory("./build/BepInEx/plugins/ksp2-aviation-units/"));
		if (configuration == "Debug")
			CopyFileToDirectory($"./ksp2-aviation-units/bin/{configuration}/netstandard2.0/ksp2-aviation-units.pdb", Directory("./build/BepInEx/plugins/ksp2-aviation-units/"));
		// var unityEditor = FindUnityEditor(2020, 3, 33, 'f');
		// if (unityEditor is null) throw new CakeException();
		// 
		// // Set unity log file location according to: https://docs.unity3d.com/Manual/LogFiles.html
		// FilePath unityLogLocation = File("");
		// if (IsRunningOnLinux()) unityLogLocation = File(@"~/.config/unity3d/Editor.log");
		// if (IsRunningOnMacOs()) unityLogLocation = File(@"~/Library/Logs/Unity/Editor.log");
		// if (IsRunningOnWindows()) unityLogLocation = ExpandEnvironmentVariables(File(@"%LOCALAPPDATA%\Unity\Editor\Editor.log"));
		// 
		// UnityEditor(unityEditor, new UnityEditorArguments
		// {
		// 	BatchMode = true,
		// 	NoGraphics = true,
		// 	Quit = true,
		// 	ProjectPath = MakeAbsolute(Directory("./ksp2-aviation-units-assets")),
		// 	ExecuteMethod = "BuildAssets.PerformBuild",
		// 	LogFile = unityLogLocation
		// }, new UnityEditorSettings
		// {
		// 	RealTimeLog = false
		// });
		// CopyDirectory("./ksp2-aviation-units-assets/Library/com.unity.addressables/aa/windows", "./build/BepInEx/plugins/ksp2-aviation-units/addressables");
	});

Task("Pack")
	.IsDependentOn("Build")
	.Does(() =>
	{
		CopyFile("./README.md", "./build/BepInEx/plugins/ksp2-aviation-units/README.md");
		CopyFile("./LICENSE.txt", "./build/BepInEx/plugins/ksp2-aviation-units/LICENSE.txt");
		Zip("./build", "build/ksp2-aviation-units.zip", "./build/**/*");
	});
	

Task("Install")
	.IsDependentOn("Build")
	.Does(() =>
	{
		var gameDir = ksp2Root != string.Empty ? ksp2Root : EnvironmentVariable("KSP2_PATH") ?? throw new ArgumentNullException("ksp2-root");
		CopyDirectory("./build/", Directory(gameDir));
	});

Task("Uninstall")
	.Does(() =>
	{
		var gameDir = ksp2Root != string.Empty ? ksp2Root : EnvironmentVariable("KSP2_PATH") ?? throw new ArgumentNullException("ksp2-root");
		DeleteDirectory(Directory(gameDir) + Directory("BepInEx/plugins/ksp2-aviation-units"), new DeleteDirectorySettings
		{
			Recursive = true
		});
	});

Task("Start")
	.IsDependentOn("Install")
	.Does(() =>
	{
		var gameDir = ksp2Root != string.Empty ? ksp2Root : EnvironmentVariable("KSP2_PATH") ?? throw new ArgumentNullException("ksp2-root");
		StartAndReturnProcess(Directory(gameDir) + File("KSP2_x64.exe"), new ProcessSettings
		{
			Arguments = ProcessArgumentBuilder.FromString("-single-instance"),
			WorkingDirectory = Directory(gameDir)
		});
	});

RunTarget(target);