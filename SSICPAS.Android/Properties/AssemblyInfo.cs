﻿using System.Reflection;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("SSICPAS.Android")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("SSICPAS.Android")]
[assembly: AssemblyCopyright("Copyright ©  2021")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]

// Configuration
#if DEVELOPMENT
[assembly: AssemblyConfiguration("Development")]
#elif DEBUG
[assembly: AssemblyConfiguration("local")]
#elif TEST
[assembly: AssemblyConfiguration("Test")]
#elif APPSTOREBETA
[assembly: AssemblyConfiguration("AppStoreBeta")]
#elif APPSTORE
[assembly: AssemblyConfiguration("AppStore")]
#else
[assembly: AssemblyConfiguration("Test")]
#endif
