// Copyright 2014-2015 Aaron Stannard, Petabridge LLC
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

using System.Threading.Tasks;
using Akka.Actor;

namespace Ripp.Sng.Lighthouse
{
    public class LighthouseService
    {
        private readonly string _actorSystemName;
        private readonly string _ipAddress;
        private readonly int? _port;

        private ActorSystem _lighthouseSystem;

        public LighthouseService() : this(null, null, null)
        {
        }

        public LighthouseService(string ipAddress, int? port, string actorSystemName)
        {
            this._ipAddress = ipAddress;
            this._port = port;
            this._actorSystemName = actorSystemName;
        }

        /// <summary>
        ///     Task completes once the Lighthouse <see cref="ActorSystem" /> has terminated.
        /// </summary>
        /// <remarks>
        ///     Doesn't actually invoke termination. Need to call <see cref="StopAsync" /> for that.
        /// </remarks>
        public Task TerminationHandle => this._lighthouseSystem.WhenTerminated;

        public void Start()
        {
            this._lighthouseSystem =
                LighthouseHostFactory.LaunchLighthouse(this._ipAddress, this._port, this._actorSystemName);
            //var pbm = PetabridgeCmd.Get(this._lighthouseSystem);
            //pbm.RegisterCommandPalette(ClusterCommands.Instance); // enable cluster management commands
            //pbm.Start();
        }

        public async Task StopAsync()
        {
            await CoordinatedShutdown.Get(this._lighthouseSystem).Run();
        }
    }
}