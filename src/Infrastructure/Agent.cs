using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Infrastructure.Domain;

namespace Infrastructure
{
    
    internal class Agent : IAgent
    {
        private readonly HtmlWeb _bot;
        private readonly HttpClient _client;

        public Agent(HtmlWeb bot, HttpClient client)
        {
            _bot = bot;
            _client = client;
        }

        public async Task<Stream> DownloadStreamAsync(Uri url, CancellationToken token)
        {
            var response = await _client.GetAsync(url, token);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();

            return stream;
        }
    }
}
