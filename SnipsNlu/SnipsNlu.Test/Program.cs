using System;

namespace SnipsNlu.Test
{
    class Program
    {
        const string rootDir = @"Data\Tests\Models\nlu_engine";
        const string zipDir = @"Data\Tests\Models\nlu_engine.zip";
        static string _sentence = "Can you make 3 cups of coffee?";

        static void Main(string[] args)
        {
            Console.WriteLine("Snips Nlu Version: " + SnipsNLUEngine.SnipsNluVersion);
            Console.WriteLine("Model Version: " + SnipsNLUEngine.GetModelVersion());
            Console.WriteLine(SnipsNLUEngine.Is64bit ? "x64" : "x86");

            using (var snipsNLUEngine = SnipsNLUEngine.CreateFromZip(zipDir))
            {
                var parsed = snipsNLUEngine.Parse(_sentence);
                Console.WriteLine(parsed);
            }

            using (var snipsNLUEngine = new SnipsNLUEngine(rootDir))
            {
                var intents = snipsNLUEngine.GetIntents(_sentence);
                var slots = snipsNLUEngine.GetSlots(_sentence, intents[0].IntentName);
                var parsed = snipsNLUEngine.Parse(_sentence);
                Console.WriteLine(parsed);
            }

            using (var snipsNLUEngine = SnipsNLUEngine.CreateFromDirectory(rootDir))
            {
                var intents = snipsNLUEngine.GetIntents(_sentence);
                var slots = snipsNLUEngine.GetSlots(_sentence, intents[0].IntentName);
                var parsed = snipsNLUEngine.Parse(_sentence);
                Console.WriteLine(parsed);
            }

            Console.WriteLine("\nDone.");
            Console.ReadKey();
        }
    }
}
