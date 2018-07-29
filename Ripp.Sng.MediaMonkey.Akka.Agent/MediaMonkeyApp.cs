using System;
using SongsDB;

namespace Ripp.Sng.MediaMonkey.Akka
{
    public class MediaMonkeyApp
    {
        public MediaMonkeyApp(SDBApplication com) => this.Com = com ?? throw new ArgumentNullException(nameof(com));

        public SDBApplication Com { get; }
    }
}