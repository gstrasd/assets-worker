using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;
using Application.Modules;
using Autofac;
using Library.Hosting;
using Library.Messaging;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Worker.Modules
{
    public class WorkerModule : Module
    {
        private readonly HostBuilderContext _context;

        public WorkerModule(HostBuilderContext context)
        {
            _context = context;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule(_context));

            builder.RegisterGeneric((context, types, parameters) =>
                {
                    var messageType = types[0];
                    var producer = context.Resolve(typeof(MessageProducer<>).MakeGenericType(messageType));
                    var consumer = context.Resolve(typeof(MessageConsumer<>).MakeGenericType(messageType));
                    var worker = Activator.CreateInstance(typeof(MessagingWorker<>).MakeGenericType(messageType), producer, consumer, context.Resolve<ILogger>());

                    return worker;
                })
                .As(typeof(MessagingWorker<>))
                .InstancePerLifetimeScope();

            var workers = (IList<string>)_context.Properties["workers"];

            if (!workers.Any() || workers.Contains("download-assets"))
            {
                builder.Register(c => c.Resolve<MessagingWorker<DownloadAssetsMessage>>())
                    .As<IHostedService>()
                    .InstancePerDependency();
            }

            if (!workers.Any() || workers.Contains("identify-assets"))
            {
                builder.Register(c => c.Resolve<MessagingWorker<IdentifyAssetsMessage>>())
                    .As<IHostedService>()
                    .InstancePerDependency();
            }
        }
    }
}
