using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace PixelBloodMod
{
    public static class ModInvoker
    {
        private static Type _EntryPointType;

        public static void OnLoad()
        {
            const string buildDirName = "build";
            const string modDllName = "PixelBloodMod.dll";
            const string entryPoint = "PixelBloodMod.EntryPoint";

            string modPath = ModAPI.Metadata.MetaLocation;
            string buildDir = Path.Combine(modPath, buildDirName);
            string modAssemblyPath = Path.Combine(buildDir, modDllName);

            Type assemblyType = Type.GetType("System.Reflection.Assembly");
            bool skipped = false;
            foreach (string file in (IEnumerable<string>)Type.GetType("System.IO.Directory").InvokeMember("EnumerateFiles", System.Reflection.BindingFlags.InvokeMethod, null, null, new object[] { buildDir, "*.dll" }))
            {
                if (skipped || file == modAssemblyPath)
                {
                    skipped = true;
                    continue;
                }
                else
                {
                    loadAssembly(file);
                }
            }

            object assembly = loadAssembly(modAssemblyPath);
            _EntryPointType = (Type)assemblyType.InvokeMember("GetType", System.Reflection.BindingFlags.InvokeMethod, null, assembly, new object[] { entryPoint, true });
            _EntryPointType.InvokeMember("OnLoad", System.Reflection.BindingFlags.InvokeMethod, null, null, null);

            object loadAssembly(string path)
            {
                return assemblyType.InvokeMember("LoadFile", System.Reflection.BindingFlags.InvokeMethod, null, null, new object[] { path });
            }
        }

        public static void Main() => _EntryPointType.InvokeMember("Main", System.Reflection.BindingFlags.InvokeMethod, null, null, null);
    }
}