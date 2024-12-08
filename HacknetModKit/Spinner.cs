using System;
using System.Threading;

namespace HacknetModKit
{
    public class Spinner : IDisposable
    {
        private const string Sequence = @"/-\|";
        private int counter;
        private int length = 0;
        private readonly int left;
        private readonly int top;
        private readonly int delay;
        private bool active;
        private bool success;
        private string status;
        private readonly Thread thread;

        public Spinner(int left, int top, int length, int delay = 100)
        {
            this.left = left;
            this.top = top;
            this.delay = delay;
            this.length = length;
            thread = new Thread(Spin);
        }

        public void Start()
        {
            Console.CursorVisible = false;
            active = true;
            if (!thread.IsAlive)
                thread.Start();
        }

        public void Stop()
        {
            active = false;
            if (success)
                Draw('*', ConsoleColor.DarkGreen);
            else
                Draw('X', ConsoleColor.DarkRed);
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(length + 6, Console.CursorTop);
            if (status != default)
                Console.Write(status);
            Console.CursorVisible = true;
            Console.WriteLine();
        }

        private void Spin()
        {
            while (active)
            {
                Turn();
                Thread.Sleep(delay);
            }
        }

        private void Draw(char c, ConsoleColor color)
        {
            Console.SetCursorPosition(left, top);
            Console.ForegroundColor = color;
            Console.Write(c);
        }

        private void Turn()
        {
            Draw(Sequence[++counter % Sequence.Length], ConsoleColor.Green);
        }

        public void Success(bool succeed = true) => success = succeed;

        public void Status(string msg) => status = msg;

        public void Dispose()
        {
            Stop();
        }
    }
}