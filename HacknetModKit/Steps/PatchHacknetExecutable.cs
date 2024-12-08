using System.IO;
using System.Linq;
using Mono.Cecil;
using static HacknetModKit.Utils.ReflectionUtils;

namespace HacknetModKit.Steps
{
    public class PatchHacknetExecutable : ExecutionStep
    {
        private const string HacknetExecutable = "Hacknet.exe";
        private const string HacknetPatchedExecutable = "HacknetPatched.exe";
        private const string HacknetOriginalExecutable = "HacknetOriginal.exe";

        protected override string Message() => "Patching Hacknet.exe...";

        protected override string DoStuff()
        {
            using (var hn = AssemblyDefinition.ReadAssembly(HacknetExecutable))
            {
                // It may have been already patched
                var definitions = hn.MainModule.Types.First(x => x.Name == "<Module>").Methods;
                if (definitions.Any(definition => definition.Name == ".cctor"))
                    return "already patched.";
            }
                
            if (!new FileInfo(Path.Combine(AssemblyDirectory, HacknetOriginalExecutable)).Exists)
                Copy(HacknetExecutable, HacknetOriginalExecutable);

            using (var hn = AssemblyDefinition.ReadAssembly(HacknetOriginalExecutable))
                PathfinderPatcher.Patch(HacknetOriginalExecutable, HacknetPatchedExecutable);
            Remove(HacknetExecutable);
            Move(HacknetPatchedExecutable, HacknetExecutable);
            return $"patched successfully, original executable backup is {HacknetOriginalExecutable}";
        }

        private static void Move(string src, string dst) =>
            new FileInfo(Path.Combine(AssemblyDirectory, src)).MoveTo(Path.Combine(AssemblyDirectory, dst));
        
        private static void Copy(string src, string dst) =>
            new FileInfo(Path.Combine(AssemblyDirectory, src)).CopyTo(Path.Combine(AssemblyDirectory, dst));

        private static void Remove(string src) =>
            new FileInfo(Path.Combine(AssemblyDirectory, src)).Delete();
    }
}