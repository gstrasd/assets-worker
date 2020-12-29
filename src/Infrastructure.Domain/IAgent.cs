using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Domain
{
    public interface IAgent
    {
        Task<Stream> DownloadStreamAsync(Uri url, CancellationToken token);
    }
}
