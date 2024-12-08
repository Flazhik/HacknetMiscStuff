using System.IO;
using System.IO.Compression;
using System.Linq;
using static HacknetModKit.Utils.ReflectionUtils;
using static HacknetModKit.Utils.ZipArchiveExtensions;

namespace HacknetModKit.Steps
{
    public class InstallBepInEx : ExecutionStep
    {
        protected override string Message() => "Installing BepInEx v6.0.0...";

        protected override string DoStuff()
        {
            var bepInVersion = BepInVersion.BepInExVersion();
            if (bepInVersion != default && bepInVersion.Major >= 6)
                return $"BepInEx v{bepInVersion} is already installed.";


            using (var archive = ZipFile.OpenRead(Path.Combine(AssemblyDirectory, "bepinex.zip")))
            {
                var bepInEx = archive.Entries.First(entry => entry.FullName == "BepInEx/");
                bepInEx.Archive.ExtractToDirectory(AssemblyDirectory, true);
            }

            return "installed.";
        }
    }
}