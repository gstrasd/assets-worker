using Autofac;
using Library.Messaging;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Infrastructure.Domain;

namespace Infrastructure.Modules
{
    public class InfrastructureModule : Module
    {
        private readonly HostBuilderContext _context;

        public InfrastructureModule(HostBuilderContext context)
        {
            _context = context;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new AwsModule(_context));

            builder.Register(c =>
                {
                    var agent = new Agent(new HtmlWeb(), new HttpClient());
                    return agent;
                })
                .As<IAgent>()
                .InstancePerDependency();
        }
    }
}
