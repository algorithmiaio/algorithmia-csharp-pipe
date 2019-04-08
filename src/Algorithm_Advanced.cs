using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Linq;
using System.Net;
using SixLabors.ImageSharp.PixelFormats;
using Algorithmia;
using AlgorithmiaPipe;
//This is an example of an algorithm. You can test it by copy/pasting the following into the web editor:
/// {"name": "Algorithmia user"}
///
/// You should expect to see the following as output:
/// {"output":"Hello Algorithmia user"}

/// 'Algo' is the primary namespace of your algorithm.
namespace AlgorithmiaPipe
{
    /// The input object type for your algorithm, please ensure that your class members are public and simple types (string, list, int, etc)
    /// 
    /// Note: json serialization is 'Case Sensitive', this means that for certain types of input - it might make sense to break
    /// from C# convention, and name your AlgoInput/AlgoOutput variables in 'snake_case' format. (https://en.wikipedia.org/wiki/Snake_case)
    public class AlgoInput : Object
    {
        public string image_path;
    }
    /// This is the output object type for your algorithm, the types defined here must be json serializable via the 'Newtonsoft.Json' package.
    public class AlgoOutput : Object
    {
        public string save_path;
    }
    /// This is the main class of your algorithm, it contains your static 'apply' method which we use as an entry point to your project.
    public class AlgorithmAdvanced
    {
        public static AlgoOutput Apply(AlgoInput input, Dictionary<string, object> context)
        {
            string file_random = Extras.RandomString(10);
            string filename = $"{file_random}.jpg";
            string localFilePath = $"/tmp/{filename}";
            string remoteFilePath = $"data://.my/collection/{filename}";
            Extras.processImage(input.image_path, localFilePath);
            FileStream stream = File.OpenRead(localFilePath);
            Algorithmia.Client client = (Algorithmia.Client)context["client"];
            client.file($"data://.my/collection/{filename}").put(stream);
            stream.Close();
            AlgoOutput output = new AlgoOutput();
            output.save_path = remoteFilePath;
            return output;
        }
        
        
        public static Dictionary<String, dynamic> OnLoad()
        {
        Dictionary<String, dynamic> context = new Dictionary<string, dynamic>();
        context["envApiKey"] = Environment.GetEnvironmentVariable("ALGORITHMIA_API_KEY");
        context["client"] = new Client(context["envApiKey"]);
        return context;
        }
        
        public static AlgorithmBuilder<AlgoInput, AlgoOutput> Configure()
        {
            AlgorithmBuilder<AlgoInput, AlgoOutput> builder = new AlgorithmBuilder<AlgoInput, AlgoOutput>();
            builder.SetLoadFunction(OnLoad);
            builder.SetApplyFunction(Apply);
            return builder;
        }
    }

    static class Extras
    {
        public static void processImage(string url, string outputname)
        {
            using (WebClient webClient = new WebClient()) 
            {
                byte [] data = webClient.DownloadData(url);

                using (MemoryStream mem = new MemoryStream(data))
                {
                    Image<Rgba32> image = Image.Load(mem);
                    image.Mutate(x => x
                        .Resize(image.Width / 2, image.Height / 2));
                    image.Save(outputname);
                }
            }
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
    
}
