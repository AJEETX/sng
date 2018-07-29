using System.Runtime.InteropServices;
using EasyConsole;
using Ripp.Sng.MediaMonkey.Akka.Agent;

namespace Ripp.Sng.MediaMonkey.Cli
{
    internal class Program
    {
        public static void ReregisterCom()
        {
            UnRegisterCom();
            RegisterCom();
        }

        public static void RegisterCom()
        {
            var rs = new RegistrationServices();
            var a = new MediaMonkeyAkkaProxy();
            rs.RegisterAssembly(typeof(MediaMonkeyAkkaProxy).Assembly,
                AssemblyRegistrationFlags.SetCodeBase);
        }

        public static void UnRegisterCom()
        {
            var rs = new RegistrationServices();
            rs.UnregisterAssembly(typeof(MediaMonkeyAkkaProxy).Assembly);
        }

        private static void Main(string[] args)
        {
            var menu = new Menu()
                .Add("Register COM", RegisterCom)
                .Add("Unregister COM", UnRegisterCom)
                .Add("Reregister COM", ReregisterCom);
            menu.Display();
        }
    }
}