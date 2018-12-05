using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Host
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "Server";
            try
            {
                var host = await StartSilo();
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();

                await host.StopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.Read();
        }

        private static async Task<ISiloHost> StartSilo()
        {
            // define the cluster configuration
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .UseDashboard()  //http://localhost:8080
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "myApp";
                })
                .AddAdoNetGrainStorageAsDefault(options =>
                {
                    options.ConnectionString = "Server=(LocalDb)\\MSSQLLocalDB;Database=OrleansDb;User Id=sa;Password=1;";
                    //options.Invariant = "MySql.Data.MySqlClient";
                    options.UseJsonFormat = true;
                })
                .Configure<EndpointOptions>(options => options.AdvertisedIPAddress = IPAddress.Loopback)
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(Grains.HelloGrain).Assembly).WithReferences())
 //.ConfigureLogging(logging => logging.AddConsole())
 ;

            var host = builder.Build();

            await host.StartAsync();
            return host;
        }
    }
}
