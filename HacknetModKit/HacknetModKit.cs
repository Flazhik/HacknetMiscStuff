using System;
using System.Collections.Generic;
using System.Linq;
using HacknetModKit.Steps;

namespace HacknetModKit
{
    internal static class HacknetModKit
    {
        private static readonly List<ExecutionStep> InstallationSteps = new List<ExecutionStep>
        {
            new DownloadBepInEx(),
            new InstallBepInEx(),
            new InstallEntrypoint(),
            new PatchHacknetExecutable(),
            new Cleanup()
        };

        public static void Main(string[] args)
        {
            if (!StartPatching())
                Environment.Exit(0);

            foreach (var _ in InstallationSteps.Where(step => !step.TryExecute()))
                ExitMessage("An error has occured during the previous installation step, aborting", 1);

            ExitMessage("\nYou're all set! Install your mods at Hacknet\\BepInEx\\plugins\nPress any key to exit.", 0);
        }

        private static void ExitMessage(string msg, int code)
        {
            if (code != 0)
                Console.ForegroundColor = ConsoleColor.Red;
            
            Console.WriteLine($@" {msg}");
            Console.ReadKey();
            Environment.Exit(code);
        }

        private static bool YN()
        {
            ConsoleKey key = default;
            while (key != ConsoleKey.Y && key != ConsoleKey.N)
                key = Console.ReadKey().Key;

            Console.WriteLine();
            return key == ConsoleKey.Y;
        }

        private static bool StartPatching()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"                                    
                  ::::::..           
                  ::.:::::::.        
                  ::     .:::::      
             .::::::::::..  .:::.    
           ::::::::::::::::.  .:::   
         :::::::..  ...::::::. .:::  
        :::::.  .:::::.  :::::. .::. 
       .::::. .::::.::::. .::::.     
  .....::::: .::.     :::. ::::. .:. 
 .:::::::::: .::.     .::. ::::.     
 .:::  :::::  :::.   .::: .::::. .:. 
  .::. .:::::  :::::::::  :::::      
   :::. .:::::.  ..... .::::::  ::.  
    :::. .::::::::::::::::::. .:::   
     .:::   ::::::::::::::.  ::::    
      .::::.   ...:::...  .::::.     
         .::::::.       .::::        
            ..:::                    
                                     ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\n Hacknet Mod Kit by Flazhik");
            Console.WriteLine("\n A big thanks to Hacknet-Pathfinder team for patching & bootstrap snippets (licensed under MIT)");
            Console.WriteLine("\n Just launch it inside Hacknet directory: it will do the rest.\n Begin? [y/n]");
            return YN();
        }
    }
}