using System;
using System.Runtime.CompilerServices;

namespace AcMultiTargetExample
{

   /// <summary>
   /// A simple example of conditional code compilation based
   /// on the target framework. The class below holds two different
   /// versions of the IsNotNull() method. One is used with .NET
   /// 4.x, and the other with .NET 8.0 or later. Note that the
   /// conditional compilation used below can be expressed more 
   /// succinctly, but is not, mainly for illustration purposes.
   /// </summary>
   
   public static partial class Check
   {

#if NET8_0_OR_GREATER

      public static void IsNotNull(object arg, [CallerArgumentExpression("arg")] string msg = "null argument")
      {
         if(arg is null)
            throw new ArgumentNullException(msg);
      }

#else      // .NET 4.x doesn't support [CallerArgumentExpression] attribute :(

      public static void IsNotNull(object arg, string msg = "null argument")
      {
         if(arg == null)
            throw new ArgumentNullException(msg);
      }

#endif

   }
}
