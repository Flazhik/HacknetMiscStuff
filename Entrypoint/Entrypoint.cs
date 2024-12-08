using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Logging;
using BepInEx.NET.Common;
using HarmonyLib;

namespace Entrypoint
{
    public static class Entrypoint
    {
        public static void Bootstrap()
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveBepAssembly;
            LoadBepInEx.Load();
        }

        public static Assembly ResolveBepAssembly(object sender, ResolveEventArgs args)
        {
            var asmName = new AssemblyName(args.Name);

            foreach (var path in Directory
                         .GetFiles("./BepInEx", $"{asmName.Name}.dll", SearchOption.AllDirectories)
                         .Select(Path.GetFullPath))
            {
                try
                {
                    return Assembly.LoadFile(path);
                }
                catch {}
            }

            return null;
        }
    }

    internal static class LoadBepInEx
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void Load()
        {
            try
            {
                Paths.SetExecutablePath("Hacknet.exe");
                AccessTools.PropertySetter(typeof(TraceLogSource), nameof(TraceLogSource.IsListening)).Invoke(null, new object[] { true });
                ConsoleManager.Initialize(true, true);
                
                var chainloader = new NetChainloader();
                chainloader.Initialize();
                chainloader.Execute();
                new Harmony("BepInEx.Plugin.HNModKit").PatchAll();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal loading exception:");
                Console.WriteLine(ex);
                Console.ReadLine();
                Environment.Exit(1);
            }
        }
    }
}