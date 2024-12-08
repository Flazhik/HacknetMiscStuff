using System.Collections.Generic;
using System.IO;
using System.Linq;
using static HacknetModKit.Utils.ReflectionUtils;

namespace HacknetModKit.Steps
{
    public class Cleanup : ExecutionStep
    {
        private static readonly List<string> Leftovers = new List<string>
        {
            "bepinex.zip",
            "bepinex_bootstrap.zip",
            "BepInEx.NET.Framework.Launcher.exe"
        };
        
        protected override string Message() => "Cleaning everything up...";

        protected override string DoStuff()
        {
            foreach (var fi in Leftovers.Select(leftover => new FileInfo(Path.Combine(AssemblyDirectory, leftover))).Where(fi => fi.Exists))
                fi.Delete();

            return "done.";
        }
    }
}