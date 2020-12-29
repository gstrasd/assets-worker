using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Domain;
using Library.Messaging;
using Serilog;

namespace Application
{
    internal class IdentifyAssetsMessageConsumer : MessageConsumer<IdentifyAssetsMessage>
    {
        private readonly ILogger _logger;

        public IdentifyAssetsMessageConsumer(ManagedChannel<IdentifyAssetsMessage> channel, ILogger logger) : base(channel)
        {
            _logger = logger;
        }

        protected override async Task ProcessAsync(IdentifyAssetsMessage message, CancellationToken token)
        {
            _logger.Debug(message.Scope);
            _logger.Debug(message.Url.ToString());
        }
    }
}