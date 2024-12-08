using BepInEx;
using BepInEx.NET.Common;
using HacknetCtrlC.Commands;
using HarmonyLib;

namespace HacknetCtrlC
{
    [BepInProcess("Hacknet.exe")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class HacknetCtrlC : BasePlugin
    {
        private static readonly Harmony Harmony = new("BepInEx.Plugin." + PluginInfo.GUID);
        
        public override void Load()
        {
            Harmony.PatchAll();
            CustomCommands.RegisterCommand(new NodesCommand());
        }
    }
}