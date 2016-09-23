using SPAD.neXt.GamePlugins.VoiceAttack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestRemoteService
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var proxy = new ServiceProxy("localhost");
                Console.WriteLine("Trying to connect ....");
                if (!proxy.TryToConnect())
                {
                    Console.WriteLine("Failed to Connect!");
                    return;
                }
                Console.WriteLine("Connected...");
                

                string tmp = "LOCAL!SYSTEM READY";
                if (args.Length > 0)
                    tmp = args[0];
                var ret = proxy.SetValue(tmp,2);
                Console.WriteLine($"Getting {tmp}");
                Console.WriteLine($"HasError: {ret.HasError} Message {ret.Error} Value {ret.Value}");
                proxy.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL: {ex.Message}");
            }
        }
    }
}
