using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("LightClaw.Engine")]
[assembly: AssemblyProduct("LightClaw.Engine")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyDescription("The core library of the LightClaw engine.")]
[assembly: AssemblyCompany("LightClaw")]
[assembly: AssemblyCopyright("Copyright 2014 © LightClaw Development Team")]
[assembly: AssemblyTrademark("LightClaw")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(true)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("37a39a9e-248f-4f2c-854b-d7d905b4a079")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

// log4net logging
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
