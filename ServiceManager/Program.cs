using System;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            (string, string)[] serviceList =
            {
                ("battle", "BattleServerSinglePipe_"),
                ("match", "MatchingServerSinglePipe_"),
                ("pubsub", "PubSubServerSinglePipe_"),
                ("trigger", "TriggerServerSinglePipe_"),
                ("web", "WebServerSinglePipe_")
            };


            if (args.Length < 2 || serviceList.Any( keyvalue => keyvalue.Item1 == args[0].ToLower()) == false )
            {
                Console.WriteLine("Argument is [ServiceName] [port] ");
                Console.WriteLine("Service List:");
                foreach (var keyvalue in serviceList)
                {
                    Console.WriteLine("\t" + keyvalue.Item1);
                }
                Console.WriteLine("web port is 0");
                return;
            }

            var pipeName = serviceList.First(keyvalue => keyvalue.Item1 == args[0].ToLower()).Item2 + args[1];

            using(var pipe = new NamedPipeClientStream(pipeName))
            {
                try
                {
                    pipe.Connect(3000);
                    if (pipe.IsConnected)
                    {
                        var msgBytes = Encoding.UTF8.GetBytes("stop");
                        var msgLenBytes = BitConverter.GetBytes(msgBytes.Length);
                        pipe.Write(msgLenBytes, 0, msgLenBytes.Length);
                        pipe.Write(msgBytes, 0, msgBytes.Length);

                        Console.WriteLine("Waiting Stop Service (max 5sec...");
                        
                        var response = pipe.ReadByte();
                        if (response == 0xAB)
                            Console.WriteLine("Service Stop Successful");
                        else
                            Console.WriteLine("Operation Failed. Please check service Process");
                        pipe.Close();

                        Task.Delay(3000).Wait();
                    }
                    else
                    {
                        Console.WriteLine("Invalid PipeName : " + pipeName);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Not running Process " + pipeName);
                }
            }
        }
    }
}
