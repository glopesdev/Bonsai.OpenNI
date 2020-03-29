# Bonsai.OpenNI
A Bonsai library for interfacing with depth cameras through [OpenNI](https://structure.io/openni).

## Usage

This library depends on [NiWrapper.Net](https://github.com/falahati/NiWrapper.Net), a .NET wrapper for OpenNI. Its NuGet package includes all the required native binaries and uses [Baseclass.Contrib.Nuget.Output](https://www.nuget.org/packages/Baseclass.Contrib.Nuget.Output/) to install them. Unfortunately Bonsai still does not support this.

To use this library you have to perfom the following:

1. Download the [NiWrapper.Net.x64](https://www.nuget.org/packages/NiWrapper.Net.x64/) package.
2. Rename its extension to `.zip`
3. Expand the compressed file.
4. Copy the contents of the `Output` folder to the root folder of Bonsai. The folder location can be found in the registry under `\HKEY_CURRENT_USER\SOFTWARE\Goncalo Lopes\Bonsai\InstallDir`.
5. There's not yet a published package of this project because its instalation using the package manager fails. You can start this project from inside Visual Studio and it will start Bonsai by passing the library in a `--lib` parameter.  