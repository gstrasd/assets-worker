using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain
{
    public class DownloadAssetsMessage
    {
        public string Scope { get; set; }
        public List<Uri> Assets { get; set; }
    }
}
