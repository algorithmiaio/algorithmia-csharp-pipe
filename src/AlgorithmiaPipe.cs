using System;
using System.Collections.Generic;
using System.Reflection;
using AlgorithmiaPipe;
using Newtonsoft.Json;

namespace AlgorithmiaPipe
{
    
    
    public class AlgorithmBuilder<I, O>     {
        
        public delegate OutputType ApplyMethod<InputType, OutputType>(InputType input, Dictionary<String, object> context);
        
        public delegate Dictionary<String, object> LoadMethod();
        public delegate void ExitMethod(Dictionary<String, object> context);

        private ApplyMethod<I, O> _applyMethod = (input, context) => {throw new Exception("you must define 'apply'.");};
        
        private Dictionary<string, object> _context = null;
        private LoadMethod _loadMethod = () => { return new Dictionary<string, object>();};
        private ExitMethod _exitMethod = context => { };
       

        public void SetLoadFunction(LoadMethod func)
        {
            _loadMethod = func;
        }

        public void SetApplyFunction(ApplyMethod<I, O> func)
        {
            _applyMethod = func;
        }

        public void SetExitFunction(ExitMethod func)
        {
            _exitMethod = func;
        }
        
        
        public static bool is_on_algorithmia()
        {
            bool result;
            try
            {
                string env = Environment.GetEnvironmentVariable("ALGORITHMIA_ENVIRONMENT");
                if (env is null)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            catch (ArgumentNullException)
            {
                result = false;
            }
            return result;
        }
        
        private void Load()
        {
            _context = _loadMethod();
            if (is_on_algorithmia())
            {
                Console.Out.WriteLine("PIPE_INIT_COMPLETE");
                Console.Out.Flush();
            }
        }

        private void Exit()
        {
            _exitMethod(_context);
        }

        private O Invoke(I input)
        {
            return _applyMethod.Invoke(input, _context);
        }
      

        private O AttemptExecute(Request request)
        {
            dynamic algorithmArguments = ValidateInput(request);
            try
            {
                return Invoke(algorithmArguments);
            }
            catch (TargetInvocationException e)
            {     
                throw e.InnerException;
            }
        }
        
        private void FaaSExecute()
        {
            string readLine;
            while ((readLine = Console.In.ReadLine()) != null)
            {
                Request request = new Request(readLine);
                object response = null;
                try
                {
                    object result = AttemptExecute(request);
                    if (result != null)
                    {
                        response = new Response(result);
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
        }
        
        private object ValidateInput(Request request)
        {
            object algorithmArguments;
            Type algorithmInputType = typeof(I);
            if (request.ContentType == "json")
            {
                algorithmArguments = JsonConvert.DeserializeObject(request.Data, algorithmInputType);
            }
            else if (request.ContentType == "text")
            {
                algorithmArguments = request.Data;
            }
            else if (request.ContentType == "binary")
            {
                byte[] binaryGlob = Convert.FromBase64String(request.Data);
                algorithmArguments = new {binaryGlob};
            }
            else
                throw new Exception($"content_type: '{request.ContentType}' is not implemented!");

            return algorithmArguments;
        }

        private void DebuggableExecute(dynamic input)
        {
            Request request;

            if (input is byte[])
            {
                string stringified = Convert.ToBase64String(input);
                request = new Request(stringified, "binary");
            }
            else if (input is string)
            {
                request = new Request(input, "text");
            }
            else
            {
                string stringified = JsonConvert.SerializeObject(input);
                request = new Request(stringified, "json");
            }

            object result = AttemptExecute(request);
            if (result != null)
            {
                Response response = new Response(result);
                Console.Out.WriteLine(JsonConvert.SerializeObject(response));
            }
            else
                throw new Exception(
                    "the response from the algorithm was 'null'. \n" +
                    "Algorithms are not allowed to return 'null'.");
        }

        public void Run(I input=default(I))
        {
            Load();
            if (is_on_algorithmia())
                {
                FaaSExecute();
            }
            else if (input != null)
            {
                DebuggableExecute(input);
            }
            else
            {
                throw new Exception("'input' must be provided when running in 'debug' mode.");
            }
            Exit();
        }
    }
}