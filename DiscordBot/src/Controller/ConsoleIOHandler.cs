using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Controller
{
    internal class ConsoleIOHandler
    {
        private readonly Queue<string> _outputQueue;
        //TODO: Find a better way to cancel this
        public CancellationTokenSource CancellationTokenSource { get; private set; }
        private readonly CancellationToken _cancellationToken;

        public ConsoleIOHandler()
        {
            _outputQueue = [];
            CancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = CancellationTokenSource.Token;
        }

        public void PrintMessage(string message)
        {
            if (Console.KeyAvailable)
            {
                _outputQueue.Enqueue(message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }

        public string? ReadInput()
        {
            return null;
        }

        public async Task<string?> PollConsole()
        {
            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    if (Console.KeyAvailable)
                    {
                        return Console.ReadLine();
                    }
                    else if (_outputQueue.Count > 0)
                    {
                        while (_outputQueue.Any())
                        {
                            Console.WriteLine(_outputQueue.Dequeue());
                        }
                    }
                }
            } catch { }

            return null;
        }
    }
}
