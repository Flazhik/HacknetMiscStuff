using System;

namespace HacknetModKit.Steps
{
    public abstract class ExecutionStep
    {
        public bool TryExecute()
        {
            Console.WriteLine($" [ ] {Message()}");
            using (var spinner = new Spinner(2, Console.CursorTop - 1, Message().Length))
            {
                try
                {
                    spinner.Start();
                    var result = DoStuff();
                    spinner.Success();
                    spinner.Status(result);
                    spinner.Dispose();

                    return true;
                }
                catch (Exception)
                {
                    spinner.Success(false);
                    spinner.Dispose();
                    return false;
                }
            }
        }
        
        protected abstract string Message();

        protected abstract string DoStuff();
    }
}