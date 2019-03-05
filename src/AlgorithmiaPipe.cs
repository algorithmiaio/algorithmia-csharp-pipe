using System;
using System.IO;

namespace AlgorithmiaPipe
{
    public static class Pipe
    {
        public static int Enter(Type algoType)
        {
            Module algoModule;
            try
            {
                algoModule = new Module(algoType);
            }
            catch (Exception e)
            {
                ExceptionResponse response = new ExceptionResponse(e);
                Write.WriteJsonToPipe(response);
                return -1;
            }
            Console.Out.WriteLine("PIPE_INIT_COMPLETE");
            Console.Out.Flush();
            while (true)
            {
                string line;
                try
                {
                    line = Console.In.ReadLine();
                }
                catch (EndOfStreamException e)
                {
                    break;
                }
                Request request = new Request(line);
                object response = null;
                try
                {
                    object result = algoModule.AttemptExecute(request);
                    if (result != null)
                    {
                        response = new Response(result, "json");
                    }
                    else
                    {
                        response = new ExceptionResponse(new Exception("the response from the algorithm was 'null'. \n" +
                                                                       "Algorithms are not allowed to return 'null'."));
                    }
                }
                catch (Exception e)
                {
                    response = new ExceptionResponse(e);
                }
                finally
                {
                    Write.WriteJsonToPipe(response);
                }
            }
            return 0;
        }
    }
}