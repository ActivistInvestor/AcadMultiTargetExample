## AcadMultiTargetExample

ActivistInvestor \ Tony T

Distributed under the terms of the MIT License

### Notice:

This file contains Autodesk-confidential information relating to AutoCAD 2027
that is restricted by NDA. It is not intended for public distribution
or disclosure to anyone that is not bound by the terms of the NDA for
AutoCAD 2027 prior to FCS.

## AcadMultiTargetExample:

A *minimal* example project that uses [Multi-targeting](https://learn.microsoft.com/en-us/visualstudio/msbuild/net-sdk-multitargeting) to target 3 different
versions of the .NET framework (.NET Framework 4.71, .NET 8.0, and .NET 10.0),
and 8 AutoCAD product releases that use those frameworks (AutoCAD 2020
through AutoCAD 2027). In order to do that without a great deal of conditional
compilation, the code must compile against the oldest targeted framework
version (in this case, .NET Framework 4.71). 
## *Prerequisites:*

See the topic below describing ***required environment variables***
that define the locations of reference assemblies for AutoCAD
releases targeting .NET 4.x, .NET 8.0, and .NET 10.0.

These environment variables are required in order for the build
logic in the included `Directory.Build.props` file to work correctly.
Without defining these environment variables, *nothing will work*.

## Multi-target C# Projects:

[Multi-targeting](https://learn.microsoft.com/en-us/visualstudio/msbuild/net-sdk-multitargeting)
provides a means for a .NET SDK-style project to target multiple
.NET framework versions. When you build a multi-target project, the
project is built *multiple times*, once for each targeted framework version, with the build output for each placed in a different sub-folder below the \Release and \Debug folders.

<center><img src="assets/buildoutput.png" width="auto" height="390"></center><br>

To use multi-targeting in a C# project, you must use the [`<TargetFrameworks>`](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#targetframeworks)
element (plural) in your .csproj file, rather than the `<TargetFramework>`
element (singular) (see [this article](https://learn.microsoft.com/en-us/dotnet/standard/frameworks) for an overview).
That page also shows [how to use the `<TargetFrameworks>` element](https://learn.microsoft.com/en-us/dotnet/standard/frameworks#how-to-specify-a-target-framework).

For example, in the .csproj file of a project that uses the *`Directory.Build.props`* and
*`Directory.Build.targets`* files included in this project, you can target
all AutoCAD releases from AutoCAD 2020 thru AutoCAD 2027 (spanning 3 different
framework versions), thusly:
```
<Project Sdk="Microsoft.NET.Sdk">
   <PropertyGroup>
      <TargetFrameworks>net471;net8.0-windows;net10.0-windows</TargetFrameworks>
   </PropertyGroup>
</Project>
```
You don't have to target all three frameworks or AutoCAD releases. For example, you can target only .NET 4.x-based releases of AutoCAD and .NET 8.0-based releases. Or, you can target only .NET 8.0- and .NET 10.0-based releases.
For example, to target only AutoCAD 2025 or later, you would use:
```
<PropertyGroup>
       <TargetFrameworks>net10.0-windows;net8.0-windows</TargetFrameworks>
</PropertyGroup>
```
Or, to target AutoCAD 2020 through AutoCAD 2026, you would use:
```
<PropertyGroup>
       <TargetFrameworks>net8.0-windows;net471</TargetFrameworks>
</PropertyGroup>
```

In the `<TargetFrameworks>` element, The first item in 
the list of target framework monikers (TFMs) determines 
the *project context*, or which framework Visual Studio 
uses for the display of item states in Solution Explorer.
	
You can easily change the *project context* framework by 
editing the `<TargetFrameworks>` element and rearranging
the order of the TFMs, so that the first item is the TFM 
of the framework used for the project context, however 
you should first ensure that the project buiilds without 
error, and there are no unsaved changes in open files.
	
It is recommended that you first unload the project, 
then open, edit, and save its .csproj file, and then 
reload the project. 

### Creating a Project that uses Multi-targeting
Visual Studio 2022 provides no way to create a multi-target project via the
'create a new project' UI. To create a new project that uses multi-targeting, 
You first create a standard classlibrary project that targets a single framework 
version, and then manually edit the .csproj file and change the `<TargetFramework>`
element to `<TargetFrameworks>` and specify the desired framework versions, and 
also add the `Directory.Build.props` and `Directory.Build.targets` files to the 
project. It is *strongly recommended* that you first *Unload* the project before
editing its .csproj file, and then after saving changes, reload the project. 

Within the `<TargetFrameworks>` element, each target framework's *Target
Framework Moniker* (TFM) must be specified, delimited by semicolons. You
can find a list of valid TFMs [here](https://learn.microsoft.com/en-us/dotnet/standard/frameworks),
although for AutoCAD development, the set of target framework monikers is
limited to the following TFMs, depending on what framework versions are
being targeted:

|Target Framework Moniker|Framework Version|Targeted AutoCAD products|
|------------------|------|-------------------------|
|`net471`|.NET 4.71|AutoCAD 2020-2024|
|`net8.0` or `net8.0-windows`|.NET 8.0|AutoCAD 2025 & 2026|
|`net10.0` or `net10.0-windows`|.NET 10.0|AutoCAD 2027 or later|


### Assembly References:

When multi-targeting is used, different references can be specified for
each targeted framework version. In the included example project, under 
the Dependencies node in Solution Explorer, you will see child nodes whose names
are the TFM of each targeted framework, and each of those child nodes will
contain assembly references that are specific to that target framework.

<center><img src="assets/references.png" width="auto" height="460"></center><br>

Specifying different AutoCAD references is required when
multi-targeting different AutoCAD product releases that use
different framework versions, as the AutoCAD assemblies that
are referenced must align with the the target framework.

Note that when multi-targeting is used with the included 
Directory.Build.props and `Directory.Build.targets` files, 
you do not have to manually add the same AutoCAD assembly 
reference to each target framework's References. You simply 
add the reference *once* as you would do with a single-target 
project. 

The build logic in the included `Directory.Build.props` and
`Directory.Build.targets`files take it from there, and
tell Visual Studio where to find the correct version 
of the assembly for each targeted framework, and ignore any
`<HintPath/>` child element specified in the `<Reference/>`
element.
	
When you add references to AutoCAD assemblies 
manually by editing the .csproj file, you can omit 
`<HintPath/>` elements as they will not be used, 
provided that the required environment variables 
are set, and the included `Directory.Build.props`
and `Directory.Build.targets` files are being used 
by the project.
	
Conversely, you can use the Visual Studio UI to
add references, and select a reference assembly
for any targeted AutoCAD release/Framework version,
as the generated`<HintPath/>`will be ignored in
any case, so You can leave it, or remove it if 
desired.

### Environment Variables

At least two or more of the following environment 
variables *must be defined* in order for the multi-target 
build logic in the included `Directory.Build.props` and 
`Directory.Build.targets` files to work correctly. 
Each of these environment variables are only required if 
you are targeting the corresponding framework. For example, 
if don't intend to target .NET 4.x in any project, then you 
don't have to define `AC_NET_4_REF_PATH`, as it will never 
be used.

You can use the `setx` command to define these environment variables,
or the Environment Variables dialog in Windows.

|Environment Variable|Description|
|-----------------|-------------|
|`AC_NET_4_REF_PATH`|Path to reference assemblies for AutoCAD 2020-2024|
|`AC_NET_8_REF_PATH`|Path to reference assemblies for AutoCAD 2025-2026|
|`AC_NET_10_REF_PATH`|Path to reference assemblies for AutoCAD 2027|

Note: The above environment variable values should not end with a 
trailing slash (\\).

### Multi-targeting Example

The included example project (AcadMultiTargetExample) targets the following framework versions and AutoCAD product releases:

|Target Framework|Target Framework Moniker (TFM)|AutoCAD Product Release(s)|
|------------|-------------|-------------|
|.NET Framework 4.71|`net471`|AutoCAD 2020 through 2024|
|.NET 8.0|`net8.0-windows`|AutoCAD 2025, 2026|
|.NET 10.0|`net10.0-windows`|AutoCAD 2027|

### Language Version:

If you target .NET 4.x, you must set `<LangVersion>` to at least 8.0 to support compilation of Nullable
Reference Types and implicit usings. Or, you can disable both of those features (as is done in this example
project) if you are primarily compiling migrated legacy code that doesn't use those features.

While you can set `<LangVersion>` to a value that's greater than the officially-supported value for .NET 4.x, you cannot use framework-dependent features (such as `Span<T>`, ranges, etc.) that were introduced in more-recent framework and C# versions, because the language features have a dependence on a more recent framework version. In some cases, packages can be added to projects targeting legacy
framework versions that add various features introduced in later framework versions to them. Examples
include the [System.Memory NuGet package](https://www.nuget.org/packages/system.memory/), which enables the use of Span<T> in older framework versions.

### `Directory.Build.props` & `Directory.Build.targets`:

The included `Directory.Build.props` and `Directory.Build.targets` files implement the multi-target build 
logic that is used by all projects in a solution, and all projects that use those files. These two files 
also serve to *vastly simplify* building AutoCAD extensions that use multi-targeting to target
multiple AutoCAD/NET framework versions.

`Directory.Build.props` and	`Directory.Build.targets` are designed to be
*fully-reusable* and are not coupled to a specific project or solution. You 
can copy and use them in other multi-target AutoCAD projects as needed, with 
no changes required.

**Important:**  Before you can use the included `Directory.Build.props` and
`Directory.Build.targets` in a project, *including this example*, **you must 
assign values to at least two of the three environment variables shown above**. 
The values of these environment variables must point to the locations of AutoCAD 
reference assemblies for each targeted framework version.

The included `Directory.Build.targets` file also adds references to the 3 basic AutoCAD assemblies that are used in most managed extensions:

```
AcMgd.dll
AcCoreMgd.dll
AcDbMgd.dll
```
Hence, you do not (and should not) add references to those assemblies to any project that uses the included `Directory.Build.targets` file.

### Building RealDwg Extensions

The `<RealDwgExtension>` property can be set to `true` in a .csproj file to prevent 
`AcMgd.dll` from being referenced, in projects where the extension is intended to 
be hosted by a RealDwg host application such as AutoCAD Core Console:

```
<PropertyGroup>
    <RealDwgExtension>true</RealDwgExtension>
</PropertyGroup>
```

### Source Code Compatibility:

Targeting releases of AutoCAD that use .NET 4.x with the same code base
that targets .NET 8.0 or later, can be problematic depending on what
features/functionality the code uses from newer frameworks and language
versions. With Windows Forms components, the problems increase exponentially.

If you can, avoid targeting older AutoCAD product releases that use .NET
4.x and you will be free from the restrictions and limitations on code that
goes with targeting that framework version. The `Directory.Build.targets`
and `Directory.Build.props` files included in this example are designed to
support targeting AutoCAD 2020 or later, but you don't have to target all
releases in multi-target projects that use these files. If you do not
specify .NET 4.x in your project's `<TargetFrameworks>` element, there will
be no build output generated for .NET 4.x, and no references for .net 4.x
needed, and that works without having to modify the `Directory.Build.props` or
`Directory.Build.targets` files. So, you can leave the Directory.Build.* files
as-is, and target only a subset of the frameworks those files support.

### Conditional Compilation:

Targeting multiple framework/AutoCAD versions from a single code base
can introduce another layer of complexity that you may need to deal with,
which is that you may have situations where you want to leverage a feature
that exists in some, but not all of the targeted framework versions.

In those cases, you can use conditional compilation with [compiler-defined preprocessor symbols](https://learn.microsoft.com/en-us/dotnet/standard/frameworks#preprocessor-symbols),
that allow you to conditionally include/exclude code depending on the
targeted framework version/AutoCAD release. The following example shows
the use of one of those preprocessor symbols with `#if/#else/#endif`, to
define two different implementations of a method, one for .NET 4.x, and
the other for .NET 8.0 or later.

```
public static partial class Check
{

#if NET8_0_OR_GREATER

   public static void IsNotNull(object arg, [CallerArgumentExpression("arg")] string msg = "null argument")
   {
      if(arg is null)
         throw new ArgumentNullException(msg).Log(msg);
   }

#else      // .NET 4.x doesn't support [CallerArgumentExpression] attribute :(

   public static void IsNotNull(object arg, string msg = "null argument")
   {
      if(arg == null)
         throw new ArgumentNullException(msg).Log(msg);
   }

#endif

}
```
### Constants Defined by `Directory.Build.props`:

The `Directory.Build.props` file included in this project define several
constants that are useful for conditional code compilation. The compiler
constants shown below can be used just like the standard compiler-defined 
preprocessor symbols (e.g., `NET_8_OR_GREATER`), and in fact, they are 
merely synonyms for same, but help to make the intent clearer.

|Symbol|Condition|
|--------------------------------|------------------------------------|
|`AUTOCAD_2020_OR_GREATER`|True when targeting AutoCAD 2020 or later|
|`AUTOCAD_2025_OR_GREATER`|True when targeting AutoCAD 2025 or later|
|`AUTOCAD_2027_OR_GREATER`|True when targeting AutoCAD 2027 or later|

Example usage (C#):
```
#if AUTOCAD_2025_OR_GREATER

   // code here will be included only when
   // targeting AutoCAD 2025/NET 8.0 or later.

#endif
```		  

### ``launchSettings.json``:

This example project includes a `launchSettings.json` file (in the 
Properties folder) that configures the project for debugging in Visual 
Studio. The launchSettings.json file defines 3 debugging profiles, one 
for each targeted framework/AutoCAD release.

You *must edit this file* and change the paths assigned to the
`executablePath`, and `workingDirectory` properties to point to 
the locations of the AutoCAD executable (e.g., acad.exe) to launch, 
for each launch profile.

You can also add command line arguments to be passed to the executable
as well in the `commandLineArgs` property. The commandLineArgs property
is defined to pass the `/nologo` switch to start AutoCAD without showing
the splash screen.

When you run the project in the debugger, you can select which launch
profile to use from the dropdown list on the Visual Studio toolbar:
<center><img src="assets/tfs.png" width="450" height="auto"></center><br>

### Converting Existing Projects to use Multi-targeting

While it's possible to convert an existing project to use multi-targeting, 
it may ultimately be better to start from scratch with a new project and copy the code files from the existing project to the new one, to ensure that the project is correctly-configured for multi-targeting.

If you prefer to convert an existing project, follow the steps below.
These steps should also be followed to create a new project that uses
multi-targeting, after first creating a new classlibrary project
that targets a single framework, and then following these same steps to 
configure the new project to use multi-targeting.

When converting an existing project, you should backup the existing project 
first. These steps assume that you've already defined the (`AC_NET_x_REF_PATH`) 
environment variables that are described above, to point to the AutoCAD 
reference assembly locations for each targeted framework.

These steps outline the process of converting an existing (or new project)
that targets a single framework version to one that targets multiple 
framework versions:

1. If the project is open in Visual Studio, right-click on the
projet node, and choose *Unload Project* from the context menu.

2. Open the project's .csproj file and change the `<TargetFramework>` 
element to `<TargetFrameworks>`, and specify the desired framework 
versions, separated by semicolons. For example, to target AutoCAD 2020 
through AutoCAD 2027 you would use:
```
<PropertyGroup>
       <TargetFrameworks>net10.0;net8.0;net471</TargetFrameworks>
</PropertyGroup>
```

3. Add copies of the `Directory.Build.props` and `Directory.Build.targets` 
files from this project to the project you are converting to multi-targeting.

4. Add a copy of the `launchSettings.json` file from this sample project to
the Properties folder of the project being converted, and edit it to have
the correct path(s) to the AutoCAD executable(s) and working directories 
for each targeted framework/AutoCAD release. Note that this step is *optional* 
and not required for multi-targeting to work.

5. Remove any references to `acmgd.dll`, `acdbmgd.dll`, and `accoremgd.dll`
from the .csproj file for the project being converted, as these three references 
are included by the `Directory.Build.targets` file.

6. If an existing project includes references to AutoCAD assemblies other
than `AcMgd.dll`, `AcDbMgd.dll`, and `AcCoreMgd.dll`, remove the `<HintPath/>`
child elements from those `<Reference>` elements, as they will not be used.
Note that this step is *optional*.

6. Finally, save all open files, and right-click on the project in Solution
Explorer and choose *Reload Project*, and then build the solution. 

### Diagnostic Console Output

When a multi-target project is built, for each targeted framework version, a diagnostic message is displayed on the output console indicating the target build and the path to the set of AutoCAD reference assemblies used to build that target. You can view the Output pane to see these messages and use them to verify that build targets are using the correct set of AutoCAD reference assemblies:

```
1>Built target for AutoCAD 2020 / .NET v4.7.1 using references from C:\Program Files\Autodesk\AutoCAD 2023
1>Built target for AutoCAD 2025 / .NET v8.0 using references from C:\Program Files\Autodesk\AutoCAD 2025
1>Built target for AutoCAD 2027 / .NET v10.0 using references from C:\Program Files\Autodesk\AutoCAD 2027
```

