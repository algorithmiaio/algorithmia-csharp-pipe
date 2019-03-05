using System;
using System.IO;
using System.IO.Pipes;
using Newtonsoft.Json;
namespace AlgorithmiaPipe
{    public class Write
    {
        private static string OutputPath = "/tmp/algoout";

        public static void WriteJsonToPipe(object response)
        {
            var client = new NamedPipeClientStream(OutputPath);
            Console.Out.Flush();
            string serialized = JsonConvert.SerializeObject(response);
            using (StreamWriter w = new StreamWriter(client))
            {
                w.Write(serialized);
                w.Write("\n");
                w.Flush();
            }
        }
    }
}