using Hacknet;
using HacknetCtrlC.Commands;
using HarmonyLib;

namespace HacknetCtrlC.Patches
{
    [HarmonyPatch(typeof(ProgramRunner))]
    public class ProgramRunnerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ProgramRunner), "ExecuteProgram")]
        public static bool ProgramRunner_ExecuteProgram_Prefix(object os_object, string[] arguments)
        {
            var arg0 = arguments[0].ToLower();
            return !CustomCommands.TryExecute(os_object, arg0);
        }
    }
}