using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Library.Hosting;
using Microsoft.Extensions.Hosting;
using Worker.Modules;

namespace Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var command = new HostRunnerCommand(BuildHostAsync);
            command.InvokeAsync(args).Wait();

#if LOCAL || DEBUG
            Console.ReadKey();
#endif
        }

        public static async Task<IHost> BuildHostAsync(IHostBuilder hostBuilder)
        {
            hostBuilder
                .UseDefaultHostConfiguration()
                .UseDefaultAppConfiguration()
                .UseAutofac((context, builder) => builder.RegisterModule(new WorkerModule(context)))
                .UseSerilog();

            var host = hostBuilder.Build();
            return host;
        }
    }
}
