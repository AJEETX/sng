using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Akka.Actor;
using Akka.Cluster;
using Akka.Configuration;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Microsoft.Win32.SafeHandles;
using Ripp.Sng.Akka.Base;
using Serilog;
using SongsDB;

namespace Ripp.Sng.MediaMonkey.Akka.Agent
{
    [ComVisible(true)] // this overrides [assembly: ComVisible(false)] (is set by project options dialog) so registry don't get bloated with unneeded types
    public class MediaMonkeyAkkaProxy : IDisposable
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const int MY_CODE_PAGE = 1250;

        public MediaMonkeyApp MmApp { get; private set; }

        public ActorSystem Akka { get; private set; }
        protected Cluster AkkaCluster { get; set; }


        public IActorRef MediaMonkeyActor { get; private set; }
        public IDependencyResolver AkkaResolver { get; private set; }


        public void Dispose()
        {
            if (this.MediaMonkeyActor != null) this.Akka.Stop(this.MediaMonkeyActor);

            this.AkkaCluster?.Leave(this.AkkaCluster.SelfAddress);

            this.Akka?.Terminate().Wait();
            this.Akka?.Dispose();
        }

        [DllImport("kernel32.dll",
            EntryPoint = "GetStdHandle",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll",
            EntryPoint = "AllocConsole",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();


        public void Init(SDBApplication mm)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.Seq("http://localhost:5341")
                .WriteTo.Console()
                .CreateLogger();
            Log.Debug("Console initialization...");
            AllocConsole();
            var stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            var safeFileHandle = new SafeFileHandle(stdHandle, true);
            var fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            var encoding = Encoding.GetEncoding(MY_CODE_PAGE);
            var standardOutput = new StreamWriter(fileStream, encoding);
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            Log.Information("Console initialized");


            Log.Debug("Inicializacije Akke");

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Ripp.Sng.MediaMonkey.Akka.Agent.akka.hocon";

            Config config;
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();
                    config = ConfigurationFactory.ParseString(result);
                }
            }

            if (config.IsEmpty) throw new Exception("Configuration not found");


            this.MmApp = new MediaMonkeyApp(mm);
            // Setup Autofac
            var builder = new ContainerBuilder();
            builder.RegisterInstance(this.MmApp);
            var container = builder.Build();

            this.Akka = SngAkkaApp.CreateAkkaSystem(config);
            this.AkkaResolver = new AutoFacDependencyResolver(container, this.Akka);

            Log.Information("Akka initialized");

            this.AkkaCluster = Cluster.Get(this.Akka);

            this.MmApp.Com.set_Objects(Assembly.GetExecutingAssembly().GetName().Name, this);

        }
    }
}