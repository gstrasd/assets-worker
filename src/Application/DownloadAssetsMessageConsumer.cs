using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Domain;
using Infrastructure.Domain;
using Library.Messaging;
using Library.Storage;
using Serilog;

namespace Application
{
    internal class DownloadAssetsMessageConsumer : MessageConsumer<DownloadAssetsMessage>
    {
        private readonly IAgent _agent;
        private readonly IStorageClient _client;
        private readonly ILogger _logger;

        public DownloadAssetsMessageConsumer(ManagedChannel<DownloadAssetsMessage> channel, IAgent agent, IStorageClient client, ILogger logger) : base(channel)
        {
            _agent = agent;
            _client = client;
            _logger = logger;
        }

        protected override async Task ProcessAsync(DownloadAssetsMessage message, CancellationToken token)
        {
            foreach (var asset in message.Assets)
            {
                var stream = await _agent.DownloadStreamAsync(asset, token);
            }
        }
    }
}
