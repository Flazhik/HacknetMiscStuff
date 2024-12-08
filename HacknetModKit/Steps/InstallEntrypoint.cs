using System.IO;
using static HacknetModKit.Utils.ReflectionUtils;

namespace HacknetModKit.Steps
{
    public class InstallEntrypoint : ExecutionStep
    {
        protected override string Message() => "Installing entrypoint DLL...";

        protected override string DoStuff()
        {
            var entryPointPath = new FileInfo(Path.Combine(AssemblyDirectory, "BepInEx/core/BepInEx.EntryPoint.dll"));
            if (entryPointPath.Exists)
                return "already installed.";
            
            using (var fs = entryPointPath.Create())
                fs.Write(Resources.HacknetModKit, 0, Resources.HacknetModKit.Length);
            
            return "installed.";
        }
    }
}