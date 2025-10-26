using System;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(AcMultiTargetExample.MyApplication))]

namespace AcMultiTargetExample
{
   /// <summary>
   /// IExtensionApplication that displays
   /// some information about the assembly
   /// and target framework used to compile
   /// the code.
   /// </summary>

   public class MyApplication : IExtensionApplication
   {
      static Assembly assembly = typeof(MyApplication).Assembly;
      public void Initialize()
      {
         Application.Idle += idle;
      }

      private void idle(object sender, EventArgs e)
      {
         Application.Idle -= idle;
         string location = assembly.Location;
         string path = Path.GetDirectoryName(location);
         string file = Path.GetFileName(location);
         string msg = $"\n{file} loaded." +
            $"\n  Assembly Location: {path}" +
            $"\n  Target framework: {TargetFrameworkName}\n";
         Application.DocumentManager.MdiActiveDocument?.
            Editor.WriteMessage(msg);
      }

      static string TargetFrameworkName
      {
         get
         {
            return assembly .GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkDisplayName 
               ?? "(TargetFrameworkAttribute not found)";
         }
      }

      public void Terminate()
      {
      }
   }

}
