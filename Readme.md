## AcadMultiTargetExample

ActivistInvestor \ Tony T

Distributed under the terms of the MIT License

- [Prerequisites](#prerequisites)
- [Introduction to Multi-targeting](#introduction-to-multi-targeting)
  * [Creating a Project that uses Multi-targeting](#creating-a-project-that-uses-multi-targeting)
  * [Assembly References](#assembly-references)
    + [The Problem](#the-problem)
    + [The Solution](#the-solution)
    + [Nuget Package References](#nuget-package-references)
    + [Project References](#project-references)
  * [Environment Variables](#environment-variables)
  * [Custom Build Logic (Directory.Build.* files)](#custom-build-logic--directorybuild--files-)
  * [Default AutoCAD References](#default-autocad-references)
    + [Building RealDwg Extensions](#building-realdwg-extensions)
  * [C# Language Issues](#c--language-issues)
    + [Source Code Compatibility](#source-code-compatibility)
    + [Conditional Compilation](#conditional-compilation)
    + [Constants Defined by `Directory.Build.props`](#constants-defined-by--directorybuildprops-)
- [Multi-targeting Example Project](#multi-targeting-example-project)
- [Debug Profiles](#debug-profiles)
- [Converting Existing Projects to use Multi-targeting](#converting-existing-projects-to-use-multi-targeting)
- [Diagnostic Console Output](#diagnostic-console-output)

<small><i><a href='http://ecotrust-canada.github.io/markdown-toc/'>Table of contents generated with markdown-toc</a></i></small>

### Notice:

This file and the materials it is included with contain Autodesk-confidential information relating to AutoCAD 2027
that is restricted by NDA. It is not intended for public distribution
or disclosure to anyone that is not bound by the terms of the NDA for
AutoCAD 2027 prior to FCS.

## AcadMultiTargetExample

A *minimal* example project that uses [Multi-targeting](https://learn.microsoft.com/en-us/visualstudio/msbuild/net-sdk-multitargeting) to target 3 different
versions of the .NET framework (.NET Framework 4.71, .NET 8.0, and .NET 10.0),
and 8 AutoCAD product releases that use those frameworks (AutoCAD 2020
through AutoCAD 2027)

## Prerequisites

See the topic below describing ***[required environment variables](#environment-variables)*** that define the locations of reference assemblies for AutoCAD
releases targeting .NET 4.x, .NET 8.0, and .NET 10.0.
These environment variables are required in order for the build
logic in the included `Directory.Build.props` file to work correctly.
Without defining these environment variables, *nothing will work*.

## Introduction to Multi-targeting

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
```xml
<Project Sdk="Microsoft.NET.Sdk">
   <PropertyGroup>
      <TargetFrameworks>net471;net8.0-windows;net10.0-windows</TargetFrameworks>
   </PropertyGroup>
</Project>
```
You don't have to target all three frameworks or AutoCAD releases. For example, you can target only .NET 4.x-based releases of AutoCAD and .NET 8.0-based releases. Or, you can target only .NET 8.0- and .NET 10.0-based releases. For example, to target only AutoCAD 2025 or later, you would use:
```xml
<PropertyGroup>
       <TargetFrameworks>net10.0-windows;net8.0-windows</TargetFrameworks>
</PropertyGroup>
```
Or, to target AutoCAD 2020 through AutoCAD 2026, you would use:
```xml
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


### Assembly References

When multi-targeting is used, different references can be specified for
each targeted framework version. In the included example project, under
the Dependencies node in Solution Explorer, you will see child nodes whose names
are the TFM of each targeted framework, and each of those child nodes will
contain assembly references that are specific to that target framework.

<center><img src="assets/references.png" width="auto" height="460"></center><br>

Using different, framework-specific AutoCAD references is
required when multi-targeting different AutoCAD product releases
that use different framework versions, which poses a problem and
is the most-complicated aspect of building multi-target AutoCAD
extensions.

#### The Problem
Unfortunately, Visual Studio and its project architecture provide
no way to reference multiple, framework-specific versions of the same 
assembly in a multi-target project. That can't be done via the UI, 
and further, a Visual Studio project has no formal representation of 
multiple, framework-specific assemblies.
Hence, getting Visual Studio to use multiple versions of framework-specific
assemblies requires custom build logic like that provided by the `Directory.Build.props` 
and `Directory.Build.targets` files included in this distribution.

When multi-targeting is used with the included `Directory.Build.props` and `Directory.Build.targets` files, you do not have to manually add different versions of the same AutoCAD assembly reference to each target framework's references. In fact, *you can't do that* using the Visual Studio UI - when you right-click on the `Dependencies` node (or any framework-specific child node) in a multi-target project and choose *Add Project Reference*, You must select an assembly having a specific path, and Visual Studio adds the assembly reference to the .csproj file, that includes a `<HintPath/>` element that specifies the absolute path to the selected assembly. By default, and without the use of the custom build logic provided in this distribution, a build will fail, because Visual Studio is incorrectly using the same version of the assembly for *all targeted frameworks*, when different versions of that assembly must be used with each target framework.

#### The Solution
The custom build logic in the included `Directory.Build.*` files *transparently* solves that problem. When that custom build logic is used in a multi-target project, You can use Visual Studio's UI to add a reference to an AutoCAD assembly to your project, in the same way you do in a single-target project, and Visual Studio will *ignore* the path specified in the `<HintPath/>` element, and will instead use the path specified in the [environment variables](#environment-variables) you've set for each target framework (described below). After you've added references to AutoCAD assemblies to a multi-target project using the Visual Studio UI, you can open the .csproj file and remove the `<HintPath/>` elements that Visual Studio generated, as they will not be used in any case. The solution takes advantage of a subtle aspect of Visual Studio's assembly resolution process, which is that it will search for an assembly reference having an absolute path specified in a `<HintPath/>` element in other locations *before* it uses the path specified in the `<HintPath/>` element. The solution simply adds the location of the AutoCAD reference assemblies for the target framework to the list of paths that Visual Studio searches before falling-back to the path specified in the `<HintPath>` element, and the correct assembly is referenced. Note that this *path-injection* process happens for each target framework in a multi-target project, which means that every target framework uses the correct corresponding AutoCAD assembly references.

You can also add references to AutoCAD assemblies manually by editing the .csproj file. If you do that, you can safely *omit* `<HintPath/>` *elements* as they *will not be used*, provided that the required [environment variables](#environment-variables) described below have been set, and the included `Directory.Build.props`
and `Directory.Build.targets` files are being used by the project.

#### Nuget Package References

You will note that this example project completely abandons any use of Autodesk-provided Nuget packages for AutoCAD. There are several reasons for this:

1.  There is no package for AutoCAD releases targeting .NET 4.x.

2.  Autodesk's NuGet packages do not support *nuget package versioning*, that allows a single package to provide multiple versions of its assemblies, one for each target framework supported by the library, or one version for each AutoCAD product release. Instead, Autodesk released completely separate packages for different AutoCAD releases based on the version of .NET which they target, and while they can generally be used across product releases that target the same version of .NET, there can be subtle API revisions across those releases that could result in issues.

3.  AutoCAD releases and the versions of .NET which they target are not fully-aligned, and without nuget semantic package versioning support, there is no mechanism that allows a package reference to specify assemblies for a specific AutoCAD product release among several that target the same version of .NET. For example, the following will not work:

```xml
      <PackageReference Include="AutoCAD.NET" Version="25.0.0" />  <!-- 2025 DLLs -->
      <PackageReference Include="AutoCAD.NET" Version="25.1.0" />  <!-- 2026 DLLs -->
```

To support all AutoCAD product releases that target the same version of .NET, the rule of thumb is to always target the ***oldest*** product release for a given .NET framework version. So, for AutoCAD releases that target .NET 4.x, that would mean compiling against the assemblies for AutoCAD 2020, and for releases that target .NET 8.0, compiling against the assemblies for AutoCAD 2025.

This example project and the included build logic is designed to support only one AutoCAD product release for each targeted .NET version, and it is your decision on what specific AutoCAD product release's assemblies should be used. You can specify that by setting the [environment variables](#environment-variables) that are used to specify the locations of the AutoCAD assemblies for each targeted framework to the appropriate locations holding the assemblies for the AutoCAD product release you want to compile against, although it is *strongly recommended* to follow the above rule-of-thumb.

For other third-party nuget packages that support package versioning and multiple target frameworks, there is no difference in how you include them in multi-target projects verses single-target projects. In other words, *It just works*. However, you should keep in mind that if you are going to target all of the AutoCAD releases from 2020-2027 and later, you should ensure that any third-party nuget packages you need to use supports all of the framework versions your project targets.

#### Project References
Project references in multi-target projects work the same way they do in single-target projects with several special requirements.

First, you don't have to specify separate project references for each target framework, but any project references must be to projects that *multi-target at least the same frameworks that are targeted by the referencing project*. In other words, if your project targets .NET 4.x, .NET 8.0, and .NET 10.0, any project references must be to projects that also multi-target those same framework versions.

### Environment Variables

The included build logic in the `Directory.Build.*` files is designed to require the developer to specify the locations of AutoCAD assemblies for each .NET framework version they wish to target. Environment variables are used to allow the `Directory.Build.*` files to be fully-portable across development environments without requiring changes.

At least two of these environment variables *must be defined* in order for the multi-target build logic to work as-designed. Each of these environment variables is required *only if you are targeting the corresponding framework*. For example, if don't intend to target .NET 4.x in any project, then you
don't have to define `AC_NET_4_REF_PATH`, as it will never be used.

You can use the `setx` command to define these environment variables,
or the Environment Variables dialog in Windows.

|Environment Variable|Description|
|-----------------|-------------|
|`AC_NET_4_REF_PATH`|Path to reference assemblies for AutoCAD 2020-2024|
|`AC_NET_8_REF_PATH`|Path to reference assemblies for AutoCAD 2025-2026|
|`AC_NET_10_REF_PATH`|Path to reference assemblies for AutoCAD 2027|

Note: The above environment variable values ***must not*** *include a trailing slash (\\)*.

Requiring paths to AutoCAD reference assemblies for each target framework to be specified using environment variables allows the `Directory.Build.*` files to be *fully-portable* across machines where the locations of the reference assemblies may differ, thereby supporting widespread distribution and team development scenarios.

### Custom Build Logic (Directory.Build.* files)

The included `Directory.Build.props` and `Directory.Build.targets` files implement the multi-target build logic that is used by all projects in a solution, and all projects that use those files. These  files serve to *vastly simplify* building AutoCAD extensions that use multi-targeting to target multiple AutoCAD/NET framework versions.

`Directory.Build.props` and	`Directory.Build.targets` are designed to be
*fully-reusable* and are not coupled to a specific project or solution. You
can copy and use them in other multi-target AutoCAD projects as needed, with
no changes required.

It is recommended that you add `Directory.Build.props` and	`Directory.Build.targets` to your Solution folder, above any project folders. Also adding them as Solution Items is optional but not required. If you add these files to your solution's folder, you don't have to add them to individual projects in the solution that are contained within the solution's folder, as they are automatically used by all projects within the folder where the files are located.

Before you can use the included `Directory.Build.props` and
`Directory.Build.targets` in a project, you must
assign values to at least two of the three `AC_NET_X_REF_PATH` [environment variables](#environment-variables) described above. The values of these environment variables must point to the locations of AutoCAD reference assemblies for each targeted framework version.

### Default AutoCAD References
The included `Directory.Build.targets` file also adds references to the 3 basic AutoCAD assemblies that are used in most managed extensions:

* `AcMgd.dll`
* `AcCoreMgd.dll`
* `AcDbMgd.dll`

Hence, you do not (and should not) add references to those assemblies to any project that uses the included `Directory.Build.targets` file.

#### Building RealDwg Extensions

The custom build logic in the included `Directory.Build.*` files recognize the `<RealDwgExtension>` property, which can be set to `true` in a .csproj file to prevent `AcMgd.dll` from being referenced, which is required in projects where the extension is to be loaded into a *RealDwg host application* such as AutoCAD Core Console:

```xml
<PropertyGroup>
    <RealDwgExtension>true</RealDwgExtension>
</PropertyGroup>
```
### C# Language Issues

If you target .NET 4.x, you can set `<LangVersion>` to at least 8.0 to support compilation of Nullable Reference Types and implicit usings. Or, you can disable both of those features (as is done in this example
project) if you are primarily compiling migrated legacy code that doesn't use those features. If you don't set `<LangVersion>` in a multi-target project, Visual Studio uses the language version officially-supported for each target framework. If you target .NET 4.x and your code uses Nullable Reference Types, Implicit Usings, or other features not supported by the language version for .NET 4.x, you will get compiler errors for the .NET 4.x build.

While you can set `<LangVersion>` to a value that's greater than the officially-supported value for .NET 4.x, you cannot use framework-dependent features (such as `Span<T>`, ranges, etc.) that were introduced in more-recent framework and C# versions, because the language features have a dependence on a more recent framework version. In some cases, packages can be added to projects targeting legacy
framework versions that add various features introduced in later framework versions to them. Examples
include the [System.Memory NuGet package](https://www.nuget.org/packages/system.memory/), which enables the use of Span<T> in older framework versions, including .NET 4.x.

#### Source Code Compatibility

Targeting releases of AutoCAD that use .NET 4.x with the same code base
that targets .NET 8.0 or later, can be problematic depending on what
features/functionality the code uses from newer frameworks and language
versions. With Windows Forms components, the issues increase exponentially.

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

#### Conditional Compilation

Targeting multiple framework/AutoCAD versions from a single code base
can introduce another layer of complexity that you may need to deal with,
which is that you may have situations where you want to leverage a feature
that exists in some, but not all of the targeted framework versions.

In those cases, you can use conditional compilation with [compiler-defined preprocessor symbols](https://learn.microsoft.com/en-us/dotnet/standard/frameworks#preprocessor-symbols),
that allow you to conditionally include/exclude code depending on the
targeted framework version/AutoCAD release. The following example shows
the use of one of those preprocessor symbols with `#if/#else/#endif`, to
define two different implementations of a method, one for .NET 4.x, and
the other for .NET 8.0 or later. Note that this example can be expressed more succinctly, but is not done that way in this case, mainly for illustration purposes.

```csharp
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
#### Constants Defined by `Directory.Build.props`

The `Directory.Build.props` file included in this project define several
constants that are useful for conditional code compilation. The compiler
constants shown below can be used just like the standard compiler-defined
preprocessor symbols (e.g., `NET_8_OR_GREATER`), and in fact, they are
merely synonyms for same, but help to make the intent clearer.

|Symbol|Condition|
|--------------------------------|------------------------------------|
|`AUTOCAD_2020_OR_GREATER`|Defined when targeting AutoCAD 2020 or later|
|`AUTOCAD_2025_OR_GREATER`|Defined when targeting AutoCAD 2025 or later|
|`AUTOCAD_2027_OR_GREATER`|Defined when targeting AutoCAD 2027 or later|

Example usage (C#):
```csharp
#if AUTOCAD_2025_OR_GREATER

   // code here will be included only when
   // targeting AutoCAD 2025/NET 8.0 or later.

#endif
```		  
## Multi-targeting Example Project

The included example project (AcadMultiTargetExample) targets the following framework versions and AutoCAD product releases:

|Target Framework|Target Framework Moniker (TFM)|AutoCAD Product Release(s)|
|------------|-------------|-------------|
|.NET Framework 4.71|`net471`|AutoCAD 2020 through 2024|
|.NET 8.0|`net8.0-windows`|AutoCAD 2025, 2026|
|.NET 10.0|`net10.0-windows`|AutoCAD 2027|

## Debug Profiles

This example project includes a `launchSettings.json` file (in the
Properties folder) that configures the project for debugging in Visual
Studio. The launchSettings.json file defines 3 debugging profiles, one
for each targeted framework/AutoCAD release.

You *must edit this file* and change the paths assigned to the
`workingDirectory` property to point to the locations of the AutoCAD
executable (e.g., acad.exe) to launch, for each launch profile. You 
can leave the `executablePath` property as-is, since Visual Studio
will look for `acad.exe` in the working directory and find it.

You can also add command line arguments to be passed to the executable
as well in the `commandLineArgs` property. The commandLineArgs property
in the example project is defined to pass the `/nologo` switch to start 
AutoCAD without showing the splash screen.

When you run the project in the debugger, you can select which launch
profile to use from the dropdown list on the Visual Studio toolbar:
<center><img src="assets/tfs.png" width="450" height="auto"></center><br>

## Converting Existing Projects to use Multi-targeting

While it's possible to convert an existing project to use multi-targeting,
it may ultimately be better to start from scratch with a new project and copy the code files from the existing project to the new one, to ensure that the project is correctly-configured for multi-targeting.

If you prefer to convert an existing project, follow the steps below.
These steps should also be followed to create a new project that uses
multi-targeting, after first creating a new classlibrary project
that targets a single framework, and then following these same steps to
configure the new project to use multi-targeting.

When converting an existing project, you should backup the existing project
first. These steps assume that you've already defined the (`AC_NET_x_REF_PATH`)
[environment variables](#environment-variables) that are described above, to point to the AutoCAD
reference assembly locations for each targeted framework.

These steps outline the process of converting an existing (or new project)
that targets a single framework version to one that targets multiple
framework versions:

1. If the project is open in Visual Studio, right-click on the
project node, and choose *Unload Project* from the context menu.

2. Open the project's .csproj file and change the `<TargetFramework>`
element to `<TargetFrameworks>`, and specify the desired framework
versions, separated by semicolons. For example, to target AutoCAD 2020
through AutoCAD 2027 you would use:
```xml
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

## Diagnostic Console Output

When a multi-target project is built, for each targeted framework version, a diagnostic message is displayed on the output console indicating the target build and the path to the set of AutoCAD reference assemblies used to build that target. You can view the Output pane to see these messages and use them to verify that build targets are using the correct set of AutoCAD reference assemblies:

```
1>Built target for AutoCAD 2020 / .NET v4.7.1 using references from C:\Program Files\Autodesk\AutoCAD 2023
1>Built target for AutoCAD 2025 / .NET v8.0 using references from C:\Program Files\Autodesk\AutoCAD 2025
1>Built target for AutoCAD 2027 / .NET v10.0 using references from C:\Program Files\Autodesk\AutoCAD 2027
```

For issues, questions, and general discussion you can visit the [repository where this project is hosted](https://github.com/ActivistInvestor/AcadMultiTargetExample).
















