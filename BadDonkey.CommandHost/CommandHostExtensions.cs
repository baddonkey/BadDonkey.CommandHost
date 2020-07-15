using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using JsonSubTypes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BadDonkey.CommandHost
{
    public static class CommandHostExtensions
    {
        private static List<ICommand>.Enumerator s_commands;

        public static List<ICommand> GetCommandsFromConfiguration(this JObject configuration, string key, JsonSerializerSettings serializerSettings)
        {
            return configuration[key].ToObject<List<ICommand>>(JsonSerializer.Create(serializerSettings));
        }

        public static IHostBuilder UseCommandHost(this IHostBuilder hostBuilder, string commandFile, string section, Assembly commandAssembly = null, JsonSerializerSettings serializerSettings = null)
        {
            var settings = serializerSettings ?? new JsonSerializerSettings();

            if (commandAssembly != null)
            {
                var converterBuilder = JsonSubtypesConverterBuilder.Of(typeof(ICommand), "kind");

                var commandTypes = commandAssembly.GetTypes().Where(s => s.IsAssignableTo<ICommand>());

                foreach (var command in commandTypes)
                {
                    converterBuilder.RegisterSubtype(command, command.Name);
                }

                settings.Converters.Add(converterBuilder.SerializeDiscriminatorProperty().Build());
            }
            
            hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            s_commands = JObject.Parse(File.ReadAllText(commandFile)).GetCommandsFromConfiguration(section, settings).GetEnumerator();

            hostBuilder.ConfigureServices((hostContext, services) =>services.AddHostedService(p => p.GetService<ICommandProcessor>()));

            hostBuilder.ConfigureContainer<ContainerBuilder>((y, b) =>
            {
                var commandList = new CommandList();

                while(s_commands.MoveNext())
                {
                    var command = s_commands.Current;

                    if (command == null)
                        return;

                    commandList.Add(command);

                    b.Register(c => Convert.ChangeType(commandList.Current, command.GetType())).As(command.GetType());

                }

                b.RegisterBuildCallback(x => AutoFacContainerProvider.Container = x);
                
                b.Register(s => commandList).SingleInstance();

                b.RegisterType<CommandProcessor>().As<ICommandProcessor>().SingleInstance();

                if (commandAssembly != null)
                    b.RegisterAssemblyTypes(commandAssembly).Where(x => x.Name.EndsWith("Handler")).Named(s => s.Name.Split("Handler").First(), typeof(ICommandHandler));
            });

            return hostBuilder;
        }
    }
}
