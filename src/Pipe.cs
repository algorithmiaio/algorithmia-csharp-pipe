using System.Collections.Generic;

namespace AlgorithmiaPipe
{
    public class Pipe
    {
        public static void Main(string[] args)
        {
            AlgorithmBuilder<string, string> builder =AlgorithmBasic.Configure();
            string input = "John";
            builder.Run(input);
        }
    }
}