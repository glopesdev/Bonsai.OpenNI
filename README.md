# Bonsai.OpenNI
A Bonsai library for interfacing with depth cameras through [OpenNI](https://structure.io/openni).

## Install

This library depends on [NiWrapper.Net](https://github.com/falahati/NiWrapper.Net), a .NET wrapper for OpenNI. Its NuGet package includes all the required native binaries and uses [Baseclass.Contrib.Nuget.Output](https://www.nuget.org/packages/Baseclass.Contrib.Nuget.Output/) to install them. Unfortunately Bonsai still does not support this.

To use this library you have to perfom the following:

1. Download the [NiWrapper.Net.x64](https://www.nuget.org/packages/NiWrapper.Net.x64/) package.
2. Rename its extension to `.zip`
3. Expand the compressed file.
4. Copy the contents of the `Output` folder to the root folder of Bonsai. The folder location can be found in the registry under `\HKEY_CURRENT_USER\SOFTWARE\Goncalo Lopes\Bonsai\InstallDir`.
5. There's not yet a published package of this project because its instalation using the package manager fails. You can start this project from inside Visual Studio and it will start Bonsai by passing the library in a `--lib` parameter.  

## Usage

1. From the toolbox, under `Source/OpenNI`, drag a `Device` into the workspace.
2. In the properties, set the `Index` to the required value. Opening the drop down shows the list of available devices.
3. From the toolbox, under `Combinator/OpenNI`, drag a `VideoStream` into the workspace. Make sure it connects to the `Device`.
4. In the properties, set `Sensor Type` to the type of stream you want (`Depth`, `Ir` or `Color`). Make sure to set the properties, under the `Video Mode`category, to values supported by the physical device.
5. A `Device` may support more than one `VideoStream` connected simultaneously.