using System.Collections.Generic;
using System.Linq;

namespace HacknetCtrlC.Commands
{
    public static class CustomCommands
    {
        private static readonly List<CustomCommand> Commands = new List<CustomCommand>();

        public static void RegisterCommand(CustomCommand command) => Commands.Add(command);

        public static bool TryExecute(object osObj, string alias)
        {
            var command = Commands.FirstOrDefault(c => c.GetAliases().Contains(alias));
            if (command == default)
                return false;
            
            command.Execute(osObj);
            return true;
        }
    }
}