#if NET461
using Serilog;
using Topshelf;

#endif

namespace Ripp.Sng.Lighthouse
{
    public partial class Program
    {
#if NET461
        public static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger = log;
            HostFactory.Run(x =>
            {
                x.SetServiceName("SngLighthouse");
                x.SetDisplayName("Sng Lighthouse");
                x.SetDescription("Seed node for the Akka Cluster");

                x.UseAssemblyInfoForServiceInfo();
                x.RunAsLocalSystem();
                x.StartAutomatically();

                x.Service<LighthouseService>(sc =>
                {
                    sc.ConstructUsing(() => new LighthouseService());

                    // the start and stop methods for the service
                    sc.WhenStarted(s => s.Start());
                    sc.WhenStopped(s => s.StopAsync().Wait());
                });

                x.EnableServiceRecovery(r => r.RestartService(1));
            });
        }
#endif
    }
}