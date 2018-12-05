using IGrains;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using System;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        const int initializeAttemptsBeforeFailing = 5;
        private static int attempt = 0;

        static async Task Main(string[] args)
        {
            Console.Title = "Client";
            Console.WriteLine("enter any key to begin");
            Console.Read();
            using (var client = await StartClientWithRetries())
            {
                while (true)
                {
                    Console.WriteLine("Input to say");

                    var input = Console.ReadLine();
                    if (!string.IsNullOrEmpty(input))
                    {
                        var grain = client.GetGrain<IHello>(10);
                        var ret = await grain.SayHello(input);
                        Console.WriteLine(ret);
                    }
                }
            }
  
        }

        private static async Task<IClusterClient> StartClientWithRetries()
        {
            attempt = 0;
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "myApp";
                })
                //.ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect(RetryFilter);
            Console.WriteLine("Client successfully connect to silo host");
            return client;
        }

        private static async Task<bool> RetryFilter(Exception exception)
        {
            if (exception.GetType() != typeof(SiloUnavailableException))
            {
                Console.WriteLine($"Cluster client failed to connect to cluster with unexpected error.  Exception: {exception}");
                return false;
            }
            attempt++;
            Console.WriteLine($"Cluster client attempt {attempt} of {initializeAttemptsBeforeFailing} failed to connect to cluster.  Exception: {exception}");
            if (attempt > initializeAttemptsBeforeFailing)
            {
                return false;
            }
            await Task.Delay(TimeSpan.FromSeconds(4));
            return true;
        }
    }
}
