using System;
using System.Configuration;
using Akka.Actor;
using Akka.Configuration;
using Akka.Configuration.Hocon;

namespace Ripp.Sng.Akka.Base
{
	public static class SngAkkaApp
	{
		public static ActorSystem CreateAkkaSystem(bool skipLoadingAkkaHocon)
		{
			if (skipLoadingAkkaHocon)
			{
#if NET461
				var section = (AkkaConfigurationSection) ConfigurationManager.GetSection("akka") ??
							  new AkkaConfigurationSection();
				var config = section.AkkaConfig;
				if (config.IsEmpty) throw new Exception("Configuration not found");
				var system = ActorSystem.Create("sng", config);
#else
				var system = ActorSystem.Create("sng");

#endif
				return system;
			}
			else
			{
				var config = AkkaConfigHelper.Load();
				var system = ActorSystem.Create("sng", config);
				return system;
			}
		}

		public static ActorSystem CreateAkkaSystem(Config config)
		{
			if (config == null) throw new ArgumentNullException(nameof(config));
			if (config.IsEmpty) throw new ArgumentNullException(nameof(config));
			var system = ActorSystem.Create("sng", config);
			return system;
		}
	}
}