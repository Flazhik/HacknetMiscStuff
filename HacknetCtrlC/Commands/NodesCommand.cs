using HacknetCtrlC.Utils;

namespace HacknetCtrlC.Commands
{
    public class NodesCommand : CustomCommand
    {
        private static readonly string[] Aliases = { "netmap" };
        
        public override string[] GetAliases() => Aliases;

        public override void Execute(object osObj)
        {
            ReflectionUtils.OsWrite(osObj, "Discovered nodes:");
            foreach (var host in ReflectionUtils.GetVisibleHosts(osObj))
                ReflectionUtils.OsWrite(osObj, $"{host.Ip}{new string(' ', 20 - host.Ip.Length)}{host.Name}");
        }
    }
}