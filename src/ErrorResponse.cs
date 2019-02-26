using System;
namespace Pipe
{
    public class ExceptionResponse
    {
        public readonly string message;
        public readonly string stack_trace;
        public readonly string error_type;

        public ExceptionResponse(Exception e)
        {
            message = e.Message;
            stack_trace = e.StackTrace;
            error_type = e.GetType().ToString();
        }
    }
}