using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Android.App;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("LFLens.Android")]
[assembly: AssemblyDescription("LF Lens is a free app that describes as text what’s seen in a picture. You can take a picture from the camera or import from the device gallery. The picture and the description can be shared through Google Drive. You can save the pictures and the description to Google Drive for archival. The app has NO third-party trackers and shares no data with third-parties. It uses Microsoft Azure Cognitive services for identifying the objects in the pictures taken. LF Lens is built on Microsoft Xamarin Framework, and is made available as open-source at github.com/littlefeetlab.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Little Feet Services Pvt Ltd")]
[assembly: AssemblyProduct("LFLens.Android")]
[assembly: AssemblyCopyright("© Copyright Little Feet Services Pvt Ltd")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

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
[assembly: Application(Icon = "@drawable/littlefeet_logo")]


// Add some common permissions, these can be removed if not needed
[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
