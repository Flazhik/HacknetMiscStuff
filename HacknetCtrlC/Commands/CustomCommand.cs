namespace HacknetCtrlC.Commands
{
    public abstract class CustomCommand
    {
        public abstract string[] GetAliases();

        public abstract void Execute(object osObj);
    }
}