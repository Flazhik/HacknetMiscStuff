using System;
using System.IO;
using System.Reflection;
using HacknetModKit.Utils;
using Mono.Cecil;

namespace HacknetModKit
{
    public static class BepInVersion
    {
        private static readonly string BepInCore =
            Path.Combine(Assembly.GetExecutingAssembly().GetDirectoryPath(), "BepInEx\\core\\BepInEx.Core.dll");
        
        public static Version BepInExVersion()
        {
            if (!new FileInfo(BepInCore).Exists)
                return default;

            using (var hn = AssemblyDefinition.ReadAssembly(Path.Combine(Assembly.GetExecutingAssembly().Location, BepInCore)))
                return hn.Name.Version;
        }
    }
}