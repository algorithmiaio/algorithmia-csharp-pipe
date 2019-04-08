using System;
using System.Collections.Generic;
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

        public Response(dynamic result)
        {
            string content;
            object data;
            if (result is string)
            {
                content = "text";
                data = result;
            } else if (result is byte[])
            {
                content = "binary";
                data = Convert.ToBase64String((byte[])result);
            }
            else
            {
                content = "json";
                data = result;
            }
            
            metadata = new MetaData(content);
            this.result = data;
        }
    }
}