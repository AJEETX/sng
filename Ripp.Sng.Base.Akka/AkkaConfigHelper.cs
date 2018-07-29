using System.IO;
using Akka.Configuration;

namespace Ripp.Sng.Akka.Base
{
    public class AkkaConfigHelper
    {
        public static Config Load()
        {
            var clusterConfig = ConfigurationFactory.ParseString(File.ReadAllText("akka.hocon"));
            return clusterConfig;
        }
    }
}