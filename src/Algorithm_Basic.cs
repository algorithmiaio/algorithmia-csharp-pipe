using System;
using System.Collections.Generic;

namespace AlgorithmiaPipe
{
        public class AlgorithmBasic
        {
            public static string Foo(string input, Dictionary<String, object> context = null)
            {
                return $"Hello {input}";
            }

            public static AlgorithmBuilder<string, string> Configure()
            {
                AlgorithmBuilder<string, string> builder = new AlgorithmBuilder<string, string>();
                builder.SetApplyFunction(Foo);
                return builder;
            }
    }
}