using System;
using FatureJa.Worker;

namespace FatureJa.DevelopmentWorker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            LoopPrincipal.Executar(TimeSpan.FromSeconds(3));
        }
    }
}