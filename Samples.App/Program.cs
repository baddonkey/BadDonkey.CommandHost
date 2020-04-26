using Autofac;
using BadDonkey.CommandHost;
using Microsoft.Extensions.Hosting;
using Samples.Commands;

namespace Samples.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseCommandHost("./appsettings.json", typeof(PrepareCommand).Assembly)
                .ConfigureContainer<ContainerBuilder>(b => b.RegisterDecorator<SampleProcessor, ICommandProcessor>());
    }
}
