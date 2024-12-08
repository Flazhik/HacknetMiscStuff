using System;
using System.IO;
using System.Reflection;

namespace HacknetModKit.Utils
{
    public static class AssemblyGetDirExtension
    {
        public static string GetDirectoryPath(this Assembly assembly)
        {
            var filePath = new Uri(assembly.CodeBase).LocalPath;
            return Path.GetDirectoryName(filePath);            
        }
    }
}