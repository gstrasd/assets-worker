using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;
using Autofac;
using Autofac.Core;
using Infrastructure.Domain;
using Infrastructure.Modules;
using Library.Amazon;
using Library.Configuration;
using Library.Hosting;
using Library.Messaging;
using Library.Queuing;
using Library.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Application.Modules
{
    public class ApplicationModule : Module
    {
        private readonly HostBuilderContext _context;

        public ApplicationModule(HostBuilderContext context)
        {
            _context = context;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new InfrastructureModule(_context));

            builder.RegisterGeneric((context, types, parameters) =>
                {
                    var capacity = _context.Configuration.GetValue<int>("Default:Channel:Capacity");
                    var instance = Activator.CreateInstance(typeof(ManagedChannel<>).MakeGenericType(types[0]), capacity);
                    return instance;
                })
                .As(typeof(ManagedChannel<>))
                .SingleInstance();

            builder.Register((context, parameters) =>
                {
                    var channel = context.Resolve<ManagedChannel<DownloadAssetsMessage>>();
                    var configuration = _context.Configuration.GetSection("Workers:DownloadAssets:QueueClient");
                    var client = context.Resolve<IQueueClient>(new NamedParameter("configuration", configuration));
                    var dequeueCount = _context.Configuration.GetValue<int>("Default:MessageProducer:DequeueCount");
                    var producer = new QueueMessageProducer<DownloadAssetsMessage>(channel, client, dequeueCount);
                    return producer;
                })
                .As<MessageProducer<DownloadAssetsMessage>>()
                .InstancePerDependency();

            builder.Register(context =>
                {
                    var channel = context.Resolve<ManagedChannel<DownloadAssetsMessage>>();
                    var configuration = _context.Configuration.GetSection("Workers:DownloadAssets:StorageClient");
                    var client = context.Resolve<IStorageClient>(new NamedParameter("configuration", configuration));
                    var consumer = new DownloadAssetsMessageConsumer(channel, context.Resolve<IAgent>(), client, context.Resolve<ILogger>());
                    return consumer;
                })
                .As<MessageConsumer<DownloadAssetsMessage>>()
                .InstancePerDependency();

            builder.Register((context, parameters) =>
                {
                    var channel = context.Resolve<ManagedChannel<IdentifyAssetsMessage>>();
                    var configuration = _context.Configuration.GetSection("Workers:IdentifyAssets:QueueClient");
                    var client = context.Resolve<IQueueClient>(new NamedParameter("configuration", configuration));
                    var dequeueCount = _context.Configuration.GetValue<int>("Default:MessageProducer:DequeueCount");
                    var producer = new QueueMessageProducer<IdentifyAssetsMessage>(channel, client, dequeueCount);
                    return producer;
                })
                .As<MessageProducer<IdentifyAssetsMessage>>()
                .InstancePerDependency();

            builder.Register(c =>
                {
                    var channel = c.Resolve<ManagedChannel<IdentifyAssetsMessage>>();
                    var consumer = new IdentifyAssetsMessageConsumer(channel, c.Resolve<ILogger>());
                    return consumer;
                })
                .As<MessageConsumer<IdentifyAssetsMessage>>()
                .InstancePerDependency();
        }
    }
}
