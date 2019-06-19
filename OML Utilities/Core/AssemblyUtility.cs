using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OmlUtilities.Core
{
    static class AssemblyUtility
    {
        /// <summary>
        /// Global parameter which is used for determining which assemblies to load.
        /// This must be set before using any Service Studio methods.
        /// </summary>
        public static PlatformVersion PlatformVersion { get; set; }

        /// <summary>
        /// Loads an assembly by its assembly name.
        /// Takes into account the current platform version.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly to be loaded.</param>
        /// <returns>Loaded assembly.</returns>
        public static Assembly GetAssembly(string assemblyName)
        {
            string dllName = assemblyName + ".dll";
            Assembly assembly = typeof(AssemblyUtility).Assembly;
            string resourceName = _GetAssemblyResourceName(assembly, dllName);

            if (resourceName == null && PlatformVersion != null)
            {
                if (PlatformVersion == PlatformVersion.O_9_1)
                {
                    switch (assemblyName)
                    {
                        case "ICSharpCode.SharpZipLib": dllName = "ICSharpCode.SharpZipLib.0.85.5.dll"; break;
                        case "Noemax.FastInfoset": dllName = "Noemax.FastInfoset.4.1.5.dll"; break;
                        case "OutSystems.Common": dllName = "OutSystems.Common.9.1.603.0.dll"; break;
                        case "OutSystems.RuntimeCommon": dllName = "OutSystems.RuntimeCommon.9.1.603.0.dll"; break;
                    }
                }
                else if (PlatformVersion == PlatformVersion.O_10_0)
                {
                    switch (assemblyName)
                    {
                        case "ICSharpCode.SharpZipLib": dllName = "ICSharpCode.SharpZipLib.0.85.5.dll"; break;
                        case "Noemax.FastInfoset": dllName = "Noemax.FastInfoset.4.1.5.dll"; break;
                        case "OutSystems.Common": dllName = "OutSystems.Common.10.0.303.0.dll"; break;
                        case "OutSystems.RuntimeCommon": dllName = "OutSystems.RuntimeCommon.10.0.303.0.dll"; break;
                    }
                }
                else if (PlatformVersion == PlatformVersion.O_11_0)
                {
                    switch (assemblyName)
                    {
                        case "ICSharpCode.SharpZipLib": dllName = "ICSharpCode.SharpZipLib.1.1.0.dll"; break;
                        case "Noemax.FastInfoset.Net4": dllName = "Noemax.FastInfoset.Net4.18.14.5339.dll"; break;
                        case "OutSystems.Common": dllName = "OutSystems.Common.11.0.520.0.dll"; break;
                        case "OutSystems.RuntimeCommon": dllName = "OutSystems.RuntimeCommon.11.0.520.0.dll"; break;
                    }
                }
                else
                {
                    throw new AssemblyUtilityException("Platform version " + PlatformVersion.Version + " not implemented.");
                }

                if (dllName == null)
                {
                    throw new AssemblyUtilityException("Unable to find assembly \"" + assemblyName + "\" for platform version " + PlatformVersion.Version + ".");
                }

                resourceName = _GetAssemblyResourceName(assembly, dllName);
            }

            if (resourceName == null)
            {
                throw new AssemblyUtilityException("Unable to load assembly \"" + assemblyName + "\" with resource name \"" + dllName + "\".");
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        /// <summary>
        /// Returns the type of an assembly by name.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="typeName">Name of the type of the assembly.</param>
        /// <returns>Assembly type.</returns>
        public static Type GetAssemblyType(string assemblyName, string typeName)
        {
            Assembly assembly = GetAssembly(assemblyName);
            return assembly.GetType(typeName, true);
        }
        
        /// <summary>
        /// Returns the value of an instance field.
        /// </summary>
        /// <typeparam name="T">Type of the returned object.</typeparam>
        /// <param name="assemblyInstance">Assembly instance object.</param>
        /// <param name="fieldName">Name of the field to be fetched.</param>
        /// <returns>Value of the fetched field.</returns>
        public static T GetInstanceField<T>(object assemblyInstance, string fieldName)
        {
            Type assemblyType = assemblyInstance.GetType();
            BindingFlags flags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public;
            return (T)assemblyType.InvokeMember(fieldName, flags, null, assemblyInstance, null);
        }

        /// <summary>
        /// Sets the value of an instance field.
        /// </summary>
        /// <param name="assemblyInstance">Assembly instance object.</param>
        /// <param name="fieldName">Name of the field to be set.</param>
        /// <param name="value">Value to be assigned to the field.</param>
        public static void SetInstanceField(object assemblyInstance, string fieldName, object value)
        {
            Type assemblyType = assemblyInstance.GetType();
            BindingFlags flags = BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public;
            PropertyInfo propertyInfo = assemblyType.GetProperty(fieldName, flags);
            propertyInfo.SetValue(assemblyInstance, value, null);
        }

        /// <summary>
        /// Executes an instance method.
        /// </summary>
        /// <typeparam name="T">Type of the returned object.</typeparam>
        /// <param name="assemblyInstance">Assembly instance object.</param>
        /// <param name="methodName">Name of the method to be executed.</param>
        /// <param name="args">Method arguments list.</param>
        /// <returns>Result of the executed instance method.</returns>
        public static T ExecuteInstanceMethod<T>(object assemblyInstance, string methodName, object[] args = null)
        {
            Type assemblyType = assemblyInstance.GetType();
            BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public;
            return (T)assemblyType.InvokeMember(methodName, flags, null, assemblyInstance, args);
        }

        /// <summary>
        /// AssemblyUtility constructor.
        /// </summary>
        static AssemblyUtility()
        {
            // Add versioned DLL detection
            AppDomain.CurrentDomain.AssemblyResolve += (sender, bargs) => GetAssembly(new AssemblyName(bargs.Name).Name);
        }

        /// <summary>
        /// Helper for getting resource name of a given DLL.
        /// </summary>
        /// <param name="assembly">Assembly of the program.</param>
        /// <param name="dllName">Name of the DLL.</param>
        /// <returns>Resource name of the assembly.</returns>
        private static string _GetAssemblyResourceName(Assembly assembly, string dllName)
        {
            return assembly.GetManifestResourceNames().FirstOrDefault(rn => rn.EndsWith(dllName));
        }
    }
}
