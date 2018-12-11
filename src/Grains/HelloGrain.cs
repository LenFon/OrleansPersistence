using IGrains;
using Orleans;
using Orleans.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grains
{
    [StorageProvider(ProviderName = "Default")]
    public class HelloGrain : Grain<HelloState>, IHello
    {
        public async Task<string> SayHello(string greeting)
        {
            Console.WriteLine(State.Text);
            State.Text = greeting;

            await WriteStateAsync();
            return $"You said: '{greeting}', I say: Hello!";
        }
    }

    public class HelloState
    {
        public string Text { get; set; }
    }
}
