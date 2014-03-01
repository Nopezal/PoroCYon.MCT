using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using TAPI.SDK;
using TAPI.SDK.Internal;

[assembly: AssemblyTitle("tAPI SDK Library")]
[assembly: AssemblyDescription("Library for the TAPI SDK")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("PoroCYon")]
[assembly: AssemblyProduct("tAPI SDK")]
[assembly: AssemblyCopyright("Copyright © PoroCYon 2013-2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(true)]
[assembly: Guid(Consts.GUID)]
[assembly: AssemblyVersion(SdkConstants.VERSION_STRING)]
[assembly: AssemblyFileVersion(SdkConstants.VERSION_STRING)]

// hehe..
[assembly: InternalsVisibleTo("tAPI SDK Tools")]
//[assembly: InternalsVisibleTo("tAPI Extended Packer")]
//[assembly: InternalsVisibleTo("tAPI SDK Mod Builder")]
//[assembly: InternalsVisibleTo("tAPI SDK Mod Decompiler")]
