using Ripp.Sng.Akka.Base;
using Serilog;

namespace Ripp.Sng.Akka.Monitor.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger = log;

            using (var a = SngAkkaApp.CreateAkkaSystem(false))
            {
                a.WhenTerminated.Wait();
            }
        }
    }
}