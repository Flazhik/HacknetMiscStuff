using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using static HacknetModKit.Utils.ReflectionUtils;

namespace HacknetModKit.Steps
{
    public class DownloadBepInEx : ExecutionStep
    {
        private const string BepInExUrl =
            "https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.2/BepInEx-NET.Framework-net40-win-x86-6.0.0-pre.2.zip";

        private const string ExpectedBepInHash = "6E9645C5B45D3F88770361AD0B23AD198730EE8B280D6CBA5F779FF951BE023A";

        private const string OutputZip = "bepinex.zip";
        
        protected override string Message() => "Downloading BepInEx v6.0.0...";

        protected override string DoStuff()
        {
            var bepInVersion = BepInVersion.BepInExVersion();
            if (bepInVersion != default && bepInVersion.Major >= 6)
                return $"BepInEx v{bepInVersion} is already downloaded.";

            if (bepInVersion != default && bepInVersion.Major < 6)
                new DirectoryInfo(Path.Combine(AssemblyDirectory, "BepInEx")).Delete(true);

            var downloadedZip = new FileInfo(Path.Combine(AssemblyDirectory, OutputZip));
            if (downloadedZip.Exists)
            {
                using (var s = new FileStream(downloadedZip.FullName, FileMode.Open))
                {
                    var hash = BitConverter.ToString(SHA256.Create().ComputeHash(s)).Replace("-", "");
                    if (hash == ExpectedBepInHash)
                        return "already downloaded.";
                }
            }

            using (var myWebClient = new WebClient())
                myWebClient.DownloadFile(BepInExUrl, "bepinex.zip");

            return "downloaded.";
        }
    }
}