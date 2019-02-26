using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace AlgorithmiaPipe
{
    public class Response
    {
        public class MetaData
        {
            public string content_type;

            public MetaData(string contentType)
            {
                content_type = contentType;
            }
        }
        
        public readonly object result;
        public readonly MetaData metadata;

        public Response(object res, string content)
        {
            metadata = new MetaData(content);
            result = res;
        }
    }
}